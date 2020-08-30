using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.ECS
{
    public class GameStateJournalist : ICleanupSystem
    {
        private readonly SnapshotFactory snapshotFactory;
        private readonly IMatchTimeStorage matchTimeStorage;
        private readonly PredictedGameStateStorage predictedGameStateStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(GameStateJournalist ));

        public GameStateJournalist(Contexts contexts, PredictedGameStateStorage predictedGameStateStorage, 
            IMatchTimeStorage matchTimeStorage)
        {
            this.predictedGameStateStorage = predictedGameStateStorage;
            this.matchTimeStorage = matchTimeStorage;
            snapshotFactory = new SnapshotFactory(contexts.serverGame);
        }

        public void Cleanup()
        {
            float matchTime = matchTimeStorage.GetMatchTimeSec();
            FullSnapshot serializedGameState = snapshotFactory.Create(matchTime,0); 
            predictedGameStateStorage.PutPredicted(serializedGameState);
        }
    }
}