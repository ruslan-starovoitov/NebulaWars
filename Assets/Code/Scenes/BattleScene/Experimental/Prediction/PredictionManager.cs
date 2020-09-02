using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PredictionManager
    {
        private readonly PlayerPredictor playerPredictor;
        private readonly PlayerEntityComparer playerEntityComparer;
        private readonly IPingStatisticsStorage pingStatisticsStorage;
        private readonly PredictedGameStateStorage predictedGameStateStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictionManager));

        public PredictionManager(PlayerPredictor playerPredictor, PredictedGameStateStorage predictedGameStateStorage,
            PlayerEntityComparer playerEntityComparer, IPingStatisticsStorage pingStatisticsStorage)
        {
            this.playerPredictor = playerPredictor;
            this.predictedGameStateStorage = predictedGameStateStorage;
            this.playerEntityComparer = playerEntityComparer;
            this.pingStatisticsStorage = pingStatisticsStorage;
        }

        public void Reconcile(SnapshotWithLastInputId snapshotWithLastInputId, ushort playerId)
        {
            PredictedSnapshot predictedSnapshot = predictedGameStateStorage
                .GetByInputId(snapshotWithLastInputId.lastProcessedInputId);
            if (predictedSnapshot == null)
            {
                log.Error($"Не найдено предсказанное состояние. " +
                          $"lastProcessedInputId = {snapshotWithLastInputId.lastProcessedInputId}");
                return;
            }
             
            bool isPredictionCorrect = playerEntityComparer
                .IsSame(predictedSnapshot, snapshotWithLastInputId, playerId);

            if (isPredictionCorrect)
            {
                log.Debug("Правильное предсказание");
            }
            else
            {
                log.Debug("Не правильное предсказание");
            }
        }
    }
}