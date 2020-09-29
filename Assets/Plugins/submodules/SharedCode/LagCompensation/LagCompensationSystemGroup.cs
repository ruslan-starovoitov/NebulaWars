using System.Collections.Generic;
using Entitas;

namespace Plugins.submodules.SharedCode.LagCompensation
{
    /// <summary>
    /// Откатывает время на время ввода игроков и вычисляет коллизии
    /// </summary>
    public class LagCompensationSystemGroup:IExecuteSystem
    {
        private readonly Contexts contexts;
        private readonly ITimeMachine timeMachine;
        private readonly IServerSnapshotHistory serverSnapshotHistory;
        private readonly TimeTravelMap travelMap = new TimeTravelMap();
        private readonly LagCompensationSystem[] lagCompensationSystems;

        public LagCompensationSystemGroup(Contexts contexts, ITimeMachine timeMachine,
            LagCompensationSystem[] lagCompensationSystems, IServerSnapshotHistory serverSnapshotHistory)
        {
            this.contexts = contexts;
            this.timeMachine = timeMachine;
            this.lagCompensationSystems = lagCompensationSystems;
            this.serverSnapshotHistory = serverSnapshotHistory;
        }
        
        public void Execute()
        {
            timeMachine.SetActualGameState(serverSnapshotHistory.GetActualGameState());
            
            
            List<TimeTravelMap.Bucket> buckets = travelMap.RefillBuckets(contexts);
            for (int bucketIndex = 0; bucketIndex < buckets.Count; bucketIndex++)
            {
                TimeTravelMap.Bucket bucket = buckets[bucketIndex];
                ProcessBucket(bucket);
            }

            //В конце лагкомпенсации мы восстанавливаем физический мир 
            //в исходное состояние
            int tickNumber = serverSnapshotHistory.GetLastTickNumber();
            timeMachine.TravelToTime(tickNumber);
        }

        private void ProcessBucket(TimeTravelMap.Bucket bucket)
        {
            //Сдвигает 3d модели к определённому моменту времени в прошлое
            timeMachine.TravelToTime(bucket.TickNumber);

            foreach (var lagCompensationSystem in lagCompensationSystems)
            {
                //todo зачем это нужно?
                // lagCompensationSystem.PastState = pastState;
                // lagCompensationSystem.PresentState = presentState;

                foreach (ServerGameEntity entity in bucket.GameEntities)
                {
                    lagCompensationSystem.Execute(entity);
                }
            }
        }
    }
}