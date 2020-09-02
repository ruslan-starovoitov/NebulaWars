using System;
using System.Linq;
using System.Text;
using Code.Scenes.BattleScene.ECS.Systems.InputSystems;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.ECS
{
    public class PredictedSnapshotHistoryUpdater : ICleanupSystem, IInitializeSystem
    {
        private readonly SnapshotFactory snapshotFactory;
        private readonly IMatchTimeStorage matchTimeStorage;
        private readonly ILastInputIdStorage lastInputIdStorage;
        private readonly PredictedGameStateStorage predictedGameStateStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictedSnapshotHistoryUpdater));

        public PredictedSnapshotHistoryUpdater(Contexts contexts, PredictedGameStateStorage predictedGameStateStorage, 
            IMatchTimeStorage matchTimeStorage, ILastInputIdStorage lastInputIdStorage)
        {
            this.predictedGameStateStorage = predictedGameStateStorage;
            this.matchTimeStorage = matchTimeStorage;
            this.lastInputIdStorage = lastInputIdStorage;
            snapshotFactory = new SnapshotFactory(contexts.serverGame);
        }

        public void Cleanup()
        {
            Snapshot snapshot = snapshotFactory.Create();
            DateTime now = DateTime.UtcNow;
            uint lastInputId = lastInputIdStorage.GetLastInputId();
            PredictedSnapshot predictedSnapshot = new PredictedSnapshot(now, lastInputId);
            predictedSnapshot.Modify(snapshot);
            predictedGameStateStorage.PutPredicted(predictedSnapshot);
        }

        public void Initialize()
        {
            DateTime now = DateTime.UtcNow;
            uint lastInputId = 0;
            PredictedSnapshot predictedSnapshot = new PredictedSnapshot(now, lastInputId);
            predictedGameStateStorage.PutPredicted(predictedSnapshot);
        }
    }
}