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
            uint lastProcessedInputId = snapshotWithLastInputId.lastProcessedInputId;
            if (lastProcessedInputId == 0)
            {
                log.Debug("С сервера пришёл тик с пустым lastProcessedInputId");
                return;
            }
            
            PredictedSnapshot predictedSnapshot = predictedGameStateStorage.GetByInputId(lastProcessedInputId);
            if (predictedSnapshot == null)
            {
                string mes = $"Не найдено предсказанное состояние. lastProcessedInputId = {lastProcessedInputId}";
                log.Error(mes);
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
                //todo создать правильный снимок
                PredictedSnapshot correctSnapshot = new PredictedSnapshot(predictedSnapshot.dateTime, 
                    predictedSnapshot.lastInputId);
                correctSnapshot.Modify(correctSnapshot);
                
                //todo нужно заменить неправильный снимок
                predictedGameStateStorage.PutCorrect(correctSnapshot);
                
                
                //todo вызвать перегенерцию положения игрока
            }
        }
    }
}