using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using JetBrains.Annotations;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// Делает поиск снимков по времени.
    /// </summary>
    public class SearchUtil
    {
        /// <summary>
        /// tickNumber + snapshot
        /// </summary>
        private readonly SortedDictionary<int, SnapshotWithLastInputId> history;
        private readonly ILog log = LogManager.CreateLogger(typeof(SearchUtil));
        
        public SearchUtil(SortedDictionary<int, SnapshotWithLastInputId> history)
        {
            this.history = history;
        }
        
        /// <summary>
        /// Возвращает снимки по такому принципу:
        /// s0.tickTime < s1.tickTime < matchTime <= s2.tickTime < s3.tickTime  
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void GetSnapshots(float matchTime,
            out SnapshotWithTime s0, out SnapshotWithTime s1,
            out SnapshotWithTime s2, out SnapshotWithTime s3)
        {
            int? s2TickNumber = GetTickNumber(matchTime);
             //Нет тика с нужным временем
            if (s2TickNumber == null)
            {
                //Смещение времени
                var firstSavedGs = history[history.Keys.Min()];
                if (matchTime < firstSavedGs.tickTime)
                {
                    //ещё нет тиков с таким большим временем
                    //время придётся сдвинуть назад
                    throw new MatchTimeIsTooShort();
                }
                else
                {
                    //запрашиваемое время слишком маленькое.
                    //есть тики с временем современным временем
                    throw new MatchTimeIsTooLong();
                }
            }
            
            int? s3TickNumber = GetClosetTickNumber(s2TickNumber.Value, TimeShift.Later);
            if (s3TickNumber == null)
            {
                string message = $"Не хватает кадров для интерполяции. Нужно сместить время старта матча назад. " +
                                 $"p2TickNumber = {s2TickNumber.Value} " +
                                 $"p2TickTime = {history[s2TickNumber.Value].tickTime} " +
                                 $"matchTime = {matchTime}"; 
                throw new Exception(message);
            }
            
            int? s1TickNumber = GetClosetTickNumber(s2TickNumber.Value, TimeShift.Earlier);
            if (s1TickNumber == null)
            {
                throw new Exception("p1TickNumber == null");
            }
        
            int? s0TickNumber = GetClosetTickNumber(s1TickNumber.Value, TimeShift.Earlier);
            if (s0TickNumber == null)
            {
                throw new Exception("p0TickNumber == null");
            }
        
            s0 = history[s0TickNumber.Value];
            s1 = history[s1TickNumber.Value];
            s2 = history[s2TickNumber.Value];
            s3 = history[s3TickNumber.Value];
        }
        
        private enum TimeShift
        {
            Earlier = -1,
            Later = 1
        }
        
        /// <summary>
        /// todo нужно заменить на коллекцию попроще
        /// </summary>
        /// <param name="tickNumber"></param>
        /// <param name="timeShift"></param>
        /// <returns></returns>
        private int? GetClosetTickNumber(int tickNumber, TimeShift timeShift)
        {
            int hypothesisTickNumber = tickNumber+(int)timeShift;
        
            while (Mathf.Abs(tickNumber-hypothesisTickNumber)<2 && hypothesisTickNumber>=0)
            {
                if (history.ContainsKey(hypothesisTickNumber))
                {
                    return hypothesisTickNumber;
                }
        
                hypothesisTickNumber += (int) timeShift;
            }
        
            return null;
        }
    
        /// <summary>
        /// Возвращает номер тика, время которого идёт сразу за запрашиваемым временем.
        /// </summary>
        /// <param name="matchTime"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int? GetTickNumber(float matchTime)
        {
            if (history.Count == 0)
            {
                log.Debug("Буффер пока пуст.");
                return null;
            }
            
            List<SnapshotWithLastInputId> snapshots = history.Values.ToList();
            //Идём от самых новых снимков к старым
            for (int index = snapshots.Count - 1; index >= 1; index--)
            {
                var prevClientSnapshot = snapshots[index - 1];
                var neededClientSnapshot = snapshots[index];
        
                //Проверка на отсортированность снимков в коллекции
                if (neededClientSnapshot.tickNumber < prevClientSnapshot.tickNumber)
                {
                    throw new Exception("Неправильный порядок состояний мира");
                }
                
                //Вернуть, если найден необходимый промежуток
                if (prevClientSnapshot.tickTime < matchTime && matchTime <= neededClientSnapshot.tickTime)
                {
                    return neededClientSnapshot.tickNumber;
                }
            }

            string message = "История не пуста, но снимок с нужным временем не найден. " +
                             $"Кол-во снимков = {history.Count} " +
                             $"matchTime = {matchTime} " +
                             $"min saved time = {history.Values.First().tickTime} " +
                             $"max saved time = {history.Values.Last().tickTime} ";
            log.Debug(message);
            return null;
        }

        public float GetPenultimateSnapshotTickTime()
        {
            if (history.Count < 4)
            {
                throw new Exception("Ещё нельзя вызывать этот метод. Интерполяция не будет работать.");
            }
            
            List<SnapshotWithLastInputId> snapshots = history.Values.ToList();
            return snapshots[snapshots.Count-2].tickTime;
        }
    }
}