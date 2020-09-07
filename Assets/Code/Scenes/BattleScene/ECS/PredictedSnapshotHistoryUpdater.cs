using System;
using System.Linq;
using System.Text;
using Code.Scenes.BattleScene.ECS.Systems.InputSystems;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class PredictedSnapshotHistoryUpdater : ICleanupSystem, IInitializeSystem
    {
        private readonly SnapshotFactory snapshotFactory;
        private readonly IMatchTimeStorage matchTimeStorage;
        private readonly ILastInputIdStorage lastInputIdStorage;
        private readonly PredictedSnapshotsStorage predictedSnapshotsStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictedSnapshotHistoryUpdater));

        public PredictedSnapshotHistoryUpdater(Contexts contexts, PredictedSnapshotsStorage predictedSnapshotsStorage, 
            IMatchTimeStorage matchTimeStorage, ILastInputIdStorage lastInputIdStorage)
        {
            this.predictedSnapshotsStorage = predictedSnapshotsStorage;
            this.matchTimeStorage = matchTimeStorage;
            this.lastInputIdStorage = lastInputIdStorage;
            snapshotFactory = new SnapshotFactory(contexts.serverGame);
        }

        public void Cleanup()
        {
            Snapshot snapshot = snapshotFactory.Create();
            DateTime now = DateTime.UtcNow;
            uint lastInputId = lastInputIdStorage.GetLastInputId();
            float deltaTimeSec = Time.deltaTime;
            PredictedSnapshot predictedSnapshot = new PredictedSnapshot(now, lastInputId, deltaTimeSec);
            predictedSnapshot.Modify(snapshot);
            predictedSnapshotsStorage.PutPredicted(predictedSnapshot);
        }

        public void Initialize()
        {
            DateTime now = DateTime.UtcNow;
            uint lastInputId = 0;
            PredictedSnapshot predictedSnapshot = new PredictedSnapshot(now, lastInputId, 0);
            predictedSnapshotsStorage.PutPredicted(predictedSnapshot);
        }
    }
}