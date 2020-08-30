using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.EntitasCore.Prediction;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// todo слишком сложно. нужно разбить
    /// </summary>
    public class ServerGameStateBuffer:ITransformStorage, ITickNumberStorage, IGameStateHistory
    {
        private DateTime? clientMatchStartTime;
        private readonly object lockObj = new object();
        private readonly TimeSpan standardDelay = TimeSpan.FromSeconds(0.1f);
        private readonly ILog log = LogManager.CreateLogger(typeof(ServerGameStateBuffer));
        private readonly SortedList<int, FullSnapshot> buffer = new SortedList<int, FullSnapshot>();
        
        public void SetNewTransforms(TransformPackMessage message)
        {
            lock (lockObj)
            {
                int tickNumber = message.TickNumber;
                float tickStartTime = message.MatchTickStartTimeSec;
                
                if (Math.Abs(tickStartTime) < 0.01)
                {
                    log.Debug($"Тик без времени tickNumber = {tickNumber} tickStartTime = {tickStartTime}");
                }

                //Создать новое состояние если такого нет
                if (!buffer.TryGetValue(tickNumber, out var gameState))
                {
                    FullSnapshot latestFullSnapshot = buffer.Values.LastOrDefault();
                    //Тик новый?
                    if (latestFullSnapshot == null)
                    {
                        //Можно дописать в конец
                        gameState = new FullSnapshot(tickNumber, tickStartTime);
                        log.Info("Первое состояние мира");
                    }
                    else if (latestFullSnapshot.tickMatchTimeSec < tickNumber)
                    {
                        //Можно дописать в конец
                        gameState = new FullSnapshot(tickNumber, tickStartTime);
                        gameState.Copy(latestFullSnapshot);
                        // log.Debug("Правильный порядок пакетов");
                    }
                    else
                    {
                        //Тик опоздал. Нужно пересчитать все тики, которые были после него
                        log.Debug("Тик опоздал");
                        return;
                    }

                    buffer.Add(tickNumber, gameState);
                }
                
                gameState.Modify(message.transform);   
            }
        }

        /// <summary>
        /// Симуляция начинается только после заполнения буффера
        /// </summary>
        /// <returns></returns>
        public bool IsReady(out int bufferLength)
        {
            lock (lockObj)
            {
                bufferLength = buffer.Count; 
                return buffer.Count >= 4;
            }
        }

        public FullSnapshot GetActualGameState(out float matchTime)
        {
            lock (lockObj)
            {
                if (!IsReady(out int bufferLength))
                {
                    string message = $"Этого вызова не должно быть. gameStateBuffer не готов." +
                                     $" {nameof(bufferLength)} = {bufferLength}";
                    throw new Exception(message);
                }

                DateTime clientNow = DateTime.UtcNow;
                matchTime = GetGameStates(clientNow, out FullSnapshot p0, out FullSnapshot p1, out FullSnapshot p2, 
                    out FullSnapshot p3);
                
                // log.Debug($"matchTime = {matchTime} p1 = {p1.tickMatchTimeSec} p2 = {p2.tickMatchTimeSec} ");
                
                if (p2.tickMatchTimeSec < p1.tickMatchTimeSec)
                {
                    throw new Exception($"Неправильный порядок тиков p1 = {p1.tickMatchTimeSec} p1 = {p2.tickMatchTimeSec}");
                }

                if (matchTime < p1.tickMatchTimeSec)
                {
                    throw new Exception($"Ошибка выбора тиков. matchTime = {matchTime} p1 = {p1.tickMatchTimeSec}");
                }
                
                if (p2.tickMatchTimeSec < matchTime)
                {
                    throw new Exception($"Ошибка выбора тиков. matchTime = {matchTime} p2 = {p2.tickMatchTimeSec}");
                }
                
                //[0, 1]
                float t = (matchTime - p1.tickMatchTimeSec) / (p2.tickMatchTimeSec - p1.tickMatchTimeSec);
                FullSnapshot result = Interpolator.Interpolate(t, p0, p1, p2, p3);
                return result;
            }
        }

        public int? GetCurrentTickNumber()
        {
            lock (lockObj)
            {
                if (!IsReady(out int bufferLength))
                {
                    log.Error($"GetCurrentTickNumber gameStateBuffer не готов. {nameof(bufferLength)} = {bufferLength}");
                    return null;
                }

                DateTime clientNow = DateTime.UtcNow;
                float matchTime = GetMatchTime(clientNow);
                return GetTickNumber(matchTime);
            }
        }

        public float GetMatchTime()
        {
            DateTime dateTime = DateTime.UtcNow;
            return GetMatchTime(dateTime);
        }
        
        private float GetMatchTime(DateTime clientNow)
        {
            //Если клиент ещё не знает своё время в матче
            if (clientMatchStartTime == null)
            {
                //Достать время самого новго тика
                float lastGameStateMatchTime = buffer.Values[buffer.Count-1].tickMatchTimeSec;
                //Найти тик который сейчас будет показан с поправкой на задержку
                float neededMatchTime = (float) (lastGameStateMatchTime - standardDelay.TotalSeconds);
                
                //Найти самый новый тик не больше нужного времени
                FullSnapshot currentFullSnapshot = null;
                int currentIndex = buffer.Count - 1;
                IList<FullSnapshot> listBuffer = buffer.Values;
                while (0 <= currentIndex)
                {
                    var gameState = listBuffer[currentIndex];
                    if (gameState.tickMatchTimeSec <= neededMatchTime)
                    {
                        currentFullSnapshot = gameState;
                        break;
                    }

                    currentIndex--;
                }

                //Задержка слишком большая. Ждём пока придёт больше тиков.
                if (currentFullSnapshot == null)
                {
                    string mes = "Ещё нет тика с нужным временем задержки. " +
                                 $"{nameof(lastGameStateMatchTime)} = {lastGameStateMatchTime} " +
                                 $"buffer.Count = {buffer.Count}";
                    throw new Exception(mes);
                }
                
                //Установка времени старта матча 
                clientMatchStartTime = clientNow - TimeSpan.FromSeconds(currentFullSnapshot.tickMatchTimeSec);
            }

            float matchTime = (float) (clientNow - clientMatchStartTime.Value).TotalSeconds;
            float serverClosestTime = buffer[buffer.Keys.Max()].tickMatchTimeSec;

            if (serverClosestTime - matchTime > 1)
            {
                log.Error("Отставание от сервера на секунду.");
                var stub = MoveToLastSavedInterval(clientNow, out _, out _, out _, out _);
                return stub;
            }

            
            return matchTime;
        }

        private float GetGameStates(DateTime clientNow, out FullSnapshot p0, out FullSnapshot p1, out FullSnapshot p2, 
            out FullSnapshot p3)
        {
            //todo если накопилось много тиков, то можно сдвинуть время матча вперёд
            float matchTime = GetMatchTime(clientNow);
            int? p2TickNumber = GetTickNumber(matchTime);


           
            
            //Нет тика с нужным временем
            if (p2TickNumber == null)
            {
                //Смещение времени
                FullSnapshot firstSavedGs = buffer[buffer.Keys.Min()];
                if (matchTime < firstSavedGs.tickMatchTimeSec)
                {
                    //нужно передвинуть время к первому промежутку
                    return MoveToFirstSavedInterval(clientNow, out p0, out p1, out p2, out p3);
                }
                else
                {
                    //нужно передвинуть время к последнему промежутку
                    return MoveToLastSavedInterval(clientNow, out p0, out p1, out p2, out p3);;
                }
            }
            
            int? p3TickNumber = GetClosetTickNumber(p2TickNumber.Value, TimeShift.Later);
            if (p3TickNumber == null)
            {
                string message = $"Не хватает кадров для интерполяции. Нужно сместить время старта матча назад. " +
                                 $"p2TickNumber = {p2TickNumber.Value} " +
                                 $"p2TickTime = {buffer[p2TickNumber.Value].tickMatchTimeSec} " +
                                 $"matchTime = {matchTime}"; 
                log.Error(message);
                return MoveToLastSavedInterval(clientNow, out p0, out p1, out p2, out p3);;
            }
            
            int? p1TickNumber = GetClosetTickNumber(p2TickNumber.Value, TimeShift.Earlier);
            if (p1TickNumber == null)
            {
                log.Error("p1TickNumber == null");
                return MoveToFirstSavedInterval(clientNow, out p0, out p1, out p2, out p3);
            }

            int? p0TickNumber = GetClosetTickNumber(p1TickNumber.Value, TimeShift.Earlier);
            if (p0TickNumber == null)
            {
                log.Error("p0TickNumber == null");
                return MoveToFirstSavedInterval(clientNow, out p0, out p1, out p2, out p3);
            }

            p0 = buffer[p0TickNumber.Value];
            p1 = buffer[p1TickNumber.Value];
            p2 = buffer[p2TickNumber.Value];
            p3 = buffer[p3TickNumber.Value];

            return matchTime;
        }

        private float MoveToLastSavedInterval(DateTime clientNow, out FullSnapshot p0, out FullSnapshot p1, 
            out FullSnapshot p2, out FullSnapshot p3)
        {
            p3 = buffer[buffer.Keys.Max()];
            p2 = buffer[GetClosetTickNumber(p3.tickNumber, TimeShift.Earlier).Value];
            p1 = buffer[GetClosetTickNumber(p2.tickNumber, TimeShift.Earlier).Value];
            p0 = buffer[GetClosetTickNumber(p1.tickNumber, TimeShift.Earlier).Value];
            float matchTime = p2.tickMatchTimeSec; 
            clientMatchStartTime = clientNow - TimeSpan.FromSeconds(matchTime);
            return matchTime;
        }

        private float MoveToFirstSavedInterval(DateTime clientNow, out FullSnapshot p0, out FullSnapshot p1, 
            out FullSnapshot p2, out FullSnapshot p3)
        {
            p0 = buffer[buffer.Keys.Min()];
            p1 = buffer[GetClosetTickNumber(p0.tickNumber, TimeShift.Later).Value];
            p2 = buffer[GetClosetTickNumber(p1.tickNumber, TimeShift.Later).Value];
            p3 = buffer[GetClosetTickNumber(p2.tickNumber, TimeShift.Later).Value];
            float matchTime = p2.tickMatchTimeSec; 
            clientMatchStartTime = clientNow - TimeSpan.FromSeconds(matchTime);
            return matchTime;
        }

        private enum TimeShift
        {
            Earlier = -1,
            Later = 1
        }
        
        private int? GetClosetTickNumber(int tickNumber, TimeShift timeShift)
        {
            int hypothesisTickNumber = tickNumber+(int)timeShift;

            while (Mathf.Abs(tickNumber-hypothesisTickNumber)<2 && hypothesisTickNumber>=0)
            {
                if (buffer.ContainsKey(hypothesisTickNumber))
                {
                    return hypothesisTickNumber;
                }

                hypothesisTickNumber += (int) timeShift;
            }

            return null;
        }
        
        private int? GetTickNumber(float matchTime)
        {
            IList<FullSnapshot> gameStates = buffer.Values;
            for (int index = gameStates.Count - 1; index >= 1; index--)
            {
                FullSnapshot prevFullSnapshot = gameStates[index - 1];
                FullSnapshot neededFullSnapshot = gameStates[index];

                if (neededFullSnapshot.tickNumber < prevFullSnapshot.tickNumber)
                {
                    throw new Exception("Неправильный порядок состояний мира");
                }
                
                if (prevFullSnapshot.tickMatchTimeSec <= matchTime && matchTime <= neededFullSnapshot.tickMatchTimeSec)
                {
                    return neededFullSnapshot.tickNumber;
                }
            }

            return null;
        }

        public int GetLastSavedTickNumber()
        {
            lock (lockObj)
            {
                if (buffer.Count == 0)
                {
                    return -1;
                }

                return buffer.Keys.Max();   
            }
        }

        public FullSnapshot GetLastShownGameState()
        {
            DateTime now = DateTime.UtcNow;
            float matchTime = GetMatchTime(now);
            lock (lockObj)
            {
                for (int index = 1; index < buffer.Values.Count; index++)
                {
                    FullSnapshot p1 = buffer.Values[index-1];
                    FullSnapshot p2 = buffer.Values[index];
                    if (p1.tickMatchTimeSec < matchTime && matchTime <= p2.tickMatchTimeSec)
                    {
                        return p2;
                    }
                }
                
                throw new NotImplementedException();
            }
        }

        public FullSnapshot Get(int serverTickNumber)
        {
            lock (lockObj)
            {
                return buffer[serverTickNumber];
            }
        }

        public FullSnapshot GetNewestGameState()
        {
            lock (lockObj)
            {
                return buffer[buffer.Keys.Max()];
            }
        }
    }
}