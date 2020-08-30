using System;
using Code.Prediction;
using Code.Scenes.BattleScene.ECS.NewSystems;
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
        
        public void Reconcile(FullSnapshot newestFullSnapshot, ushort playerId)
        {
            float pingSec = pingStatisticsStorage.GetLastPingMs()/1000;
            // string mes = $"время нового состояния = {newestFullSnapshot.tickMatchTimeSec} пинг = {pingSec}";
            // log.Info(mes);
            float matchTime = newestFullSnapshot.tickMatchTimeSec - pingSec;
            FullSnapshot predictedState = predictedGameStateStorage.GetClosestByTime(matchTime);
            if (predictedState == null)
            {
                // log.Error($"Не найдено состояние с нужным временем. matchTime = {matchTime}");
                return;
            }
            else
            {
                log.Debug("Нужное состояние найдено");
            }
            
            //если прогнозируемое состояние совпадает с последним состоянием сервера,
            //состояние сервера нужно применить к прогнозируемому игроку
            if (playerEntityComparer
                .IsSame(predictedState, newestFullSnapshot, playerId))
            {
                FullSnapshot tempState = new FullSnapshot();
                tempState.Copy(newestFullSnapshot);
                // заменить неплохо предсказанное состояние точным состоянием из сервера
                predictedGameStateStorage.PutCorrect(tempState); 
            }
            else
            {
                //todo показать разрыв соединения
                log.Debug("Разрыв соединения");
                //если предсказанное состояние игрока не совпадает с настоящим,
                //то пересимулировать положение игрока по историии ввода 
            
                //заменить предсказанное состояние на настоящее
                // GameState lastGameState = gameStateHistory.PutCorrectGameState(newestGameState);
                // for (int i = newestTickNumber; i < currentTickNumber; i++) 
                // {
                //     //пересоздать все неправильные состояния
                //     lastGameState = playerPredictor.Predict(lastGameState, playerId);
                // }
            }
        }
    }
}