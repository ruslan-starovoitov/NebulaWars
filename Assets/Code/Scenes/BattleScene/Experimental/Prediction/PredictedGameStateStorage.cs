using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PredictedGameStateStorage
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictedGameStateStorage));
        private readonly SortedDictionary<float, FullSnapshot> history = new SortedDictionary<float, FullSnapshot>();
        
        public void PutPredicted(FullSnapshot FullSnapshot)
        {
            float time = FullSnapshot.tickMatchTimeSec;
            if (!history.ContainsKey(time))
            {
                history.Add(time, FullSnapshot);
            }
            else
            {
                log.Error($"Игровое состояние с таким временем уже есть. time = {time}");
            }
        }

        public void PutCorrect(FullSnapshot FullSnapshot)
        {
            float time = FullSnapshot.tickMatchTimeSec;
            if (history.ContainsKey(time))
            {
                history[FullSnapshot.tickMatchTimeSec] = FullSnapshot;
            }
            else
            {
                log.Error("Нет состояния с таким временем");
            }
        }

        [CanBeNull]
        public FullSnapshot GetClosestByTime(float matchTime)
        {
            if (history.Count <= 1)
            {
                return null;
            }
            
            FullSnapshot earlier = null; 
            FullSnapshot later = null; 
            foreach (KeyValuePair<float, FullSnapshot> pair in history)
            {
                float tickMatchTime = pair.Key;
                if (matchTime < tickMatchTime)
                {
                    later = pair.Value;
                    break;
                }
                else
                {
                    earlier = pair.Value;
                }
            }

            if (later == null)
            {
                string message = $"Не найдено игровое состояние позже нужного. " +
                                 $"matchTime = {matchTime}" +
                                 $"min = {history.Keys.Min()} " +
                                 $"max = {history.Keys.Max()} "
                    ;
                log.Error(message);
                return null;
            }
            
            if (earlier == null)
            {
                string mes = "Этого не должно произойти так как была проверка count != 0. " + history.Count;
                throw new Exception(mes);
            }

            //Какое из состояний ближе к нужному
            float earlierDelta = matchTime - earlier.tickMatchTimeSec;
            float laterDelta = later.tickMatchTimeSec - matchTime;
            if (earlierDelta < laterDelta)
            {
                if (earlierDelta > 0.1f)
                {
                    throw new Exception("Слишком большой зазор");
                }
                
                return earlier;
            }
            else
            {
                if (laterDelta > 0.1f)
                {
                    throw new Exception("Слишком большой зазор");
                }
                
                return later;
            }
        }
    }
}