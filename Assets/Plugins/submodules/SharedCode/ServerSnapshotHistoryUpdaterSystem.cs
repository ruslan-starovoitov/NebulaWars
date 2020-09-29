using System;
using Entitas;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Plugins.submodules.SharedCode
{
    /// <summary>
    /// Делать снимок игрового состояния 
    /// </summary>
    public class ServerSnapshotHistoryUpdaterSystem : IExecuteSystem
    {
        private DateTime? matchStartTime;
        private readonly SnapshotFactory snapshotFactory;
        private readonly IGroup<ServerGameEntity> warshipsGroup;
        private readonly ITickStartTimeStorage tickStartTimeStorage;
        private readonly IServerSnapshotHistory serverSnapshotHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(ServerSnapshotHistoryUpdaterSystem));

        public ServerSnapshotHistoryUpdaterSystem(Contexts contexts, IServerSnapshotHistory serverSnapshotHistory,
            ITickStartTimeStorage tickStartTimeStorage)
        {
            this.serverSnapshotHistory = serverSnapshotHistory;
            this.tickStartTimeStorage = tickStartTimeStorage;
            snapshotFactory = new SnapshotFactory(contexts.serverGame);
        }

        public void Execute()
        {
            if (matchStartTime == null)
            {
                matchStartTime = tickStartTimeStorage.GetTickStartTime();
                log.Info($"Установка времени старта матча {matchStartTime}");
            }
            
            float matchTime = (float) (tickStartTimeStorage.GetTickStartTime() - matchStartTime.Value).TotalSeconds;
            int tickNumber = serverSnapshotHistory.GetLastTickNumber()+1;
            var snapshot = snapshotFactory.Create();
            SnapshotWithTime snapshotWithTime = new SnapshotWithTime(tickNumber, matchTime);
            snapshotWithTime.Modify(snapshot);
            serverSnapshotHistory.Add(snapshotWithTime);
        }
    }
}