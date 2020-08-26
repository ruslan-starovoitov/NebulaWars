using Plugins.submodules.SharedCode.Logger;

namespace Code.Prediction
{
    public class PredictionManager
    {
        private readonly PlayerPredictor playerPredictor;
        private readonly IGameStateHistory gameStateHistory;
        private readonly PlayerEntityComparer playerEntityComparer;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictionManager));

        public PredictionManager(PlayerPredictor playerPredictor, IGameStateHistory gameStateHistory,
            PlayerEntityComparer playerEntityComparer)
        {
            this.playerPredictor = playerPredictor;
            this.gameStateHistory = gameStateHistory;
            this.playerEntityComparer = playerEntityComparer;
        }
        
        public void Reconcile(GameState newestGameState, int currentTickNumber, ushort playerId)
        {
            // int newestTickNumber =  newestGameState.tickNumber;
            // // GameState predictedState = gameStateHistory.Get(newestTickNumber);
            //
            // //если прогнозируемое состояние совпадает с последним состоянием сервера,
            // //состояние сервера нужно применить к прогнозируемому игроку
            // if (playerEntityComparer.IsSame(predictedState, newestGameState, playerId))
            // {
            //     GameState tempState = new GameState();
            //     tempState.Copy(newestGameState);
            //     // заменить неплохо предсказанное состояние точным состоянием из сервера
            //     gameStateHistory.PutCorrectGameState(tempState); 
            // }
            // else
            // {
            //     //todo показать разрыв соединения
            //     log.Debug("Разрыв соединения");
            //     //если предсказанное состояние игрока не совпадает с настоящим,
            //     //то пересимулировать положение игрока по историии ввода 
            //
            //     //заменить предсказанное состояние на настоящее
            //     GameState lastGameState = gameStateHistory.PutCorrectGameState(newestGameState);
            //     for (int i = newestTickNumber; i < currentTickNumber; i++) 
            //     {
            //         //пересоздать все неправильные состояния
            //         lastGameState = playerPredictor.Predict(lastGameState, playerId);
            //     }
            // }
        }
    }
}