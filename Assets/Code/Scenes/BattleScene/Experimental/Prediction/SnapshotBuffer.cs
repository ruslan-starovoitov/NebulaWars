using System.Collections.Generic;
using System.Linq;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// Хранит снимки от сервера.
    /// </summary>
    public class SnapshotBuffer : ISnapshotBuffer
    {
        private readonly object lockObj = new object();
        private readonly ILog log = LogManager.CreateLogger(typeof(SnapshotBuffer));
        private readonly SortedDictionary<int, SnapshotWithLastInputId> history
            = new SortedDictionary<int, SnapshotWithLastInputId>();
        
        public int GetLength()
        {
            lock (lockObj)
            {
                return history.Count;
            }
        }

        public SnapshotWithLastInputId GetNewestSnapshot()
        {
            lock (lockObj)
            {
                return history[history.Keys.Max()];
            }
        }

        public int GetNewestTickNumber()
        {
            lock (lockObj)
            {
                return history.Keys.Max();
            }
        }

        public void Add(SnapshotWithLastInputId snapshotArg)
        {
            lock (lockObj)
            {
                // log.Info($"Добавление нового тика № = {snapshotArg.tickNumber} time = {snapshotArg.tickTime}");
                if (history.TryGetValue(snapshotArg.tickNumber, out var snapshot))
                {
                    snapshot.Modify(snapshotArg);
                }
                else
                {
                    history.Add(snapshotArg.tickNumber, snapshotArg);
                }
            }
        }

        public int? GetTickNumberByTime(float matchTime)
        {
            lock (lockObj)
            {
                return history.GetTickNumber(matchTime);
            }
        }

        public float GetPenultimateSnapshotTickTime()
        {
            lock(lockObj)
            {
                return history.GetPenultimateSnapshotTickTime();
            }
        }

        public void GetSnapshots(float matchTime, 
            out SnapshotWithTime s0, out SnapshotWithTime s1,
            out SnapshotWithTime s2, out SnapshotWithTime s3)
        {
            lock (lockObj)
            {
                history.GetSnapshots(matchTime, out s0, out s1, out s2, out s3);
            }
        }
    }
}