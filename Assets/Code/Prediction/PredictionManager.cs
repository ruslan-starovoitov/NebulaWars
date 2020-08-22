namespace Code.Prediction
{
    /// <summary>
    /// При получении нового тика от игрового сервера проверяет, что аватар игрока был правильно предсказан
    /// </summary>
    public class PredictionManager
    {
        private readonly Prediction prediction;
        private readonly GameStateCopier gameStateCopier;
        private readonly GameStateHistory localStateHistory;
        private readonly GameStateComparer gameStateComparer;

        public PredictionManager(Prediction prediction, GameStateCopier gameStateCopier,
            GameStateHistory localStateHistory, GameStateComparer gameStateComparer)
        {
            this.prediction = prediction;
            this.gameStateCopier = gameStateCopier;
            this.localStateHistory = localStateHistory;
            this.gameStateComparer = gameStateComparer;
        }
        
        public GameState Reconcile(int currentTick, ServerGameStateData serverStateData, GameState currentState, 
            ushort playerId)
        {
            GameState serverGameState =  serverStateData.GameState;
            int serverTickNumber =  serverGameState.tickNumber;
            GameState predictedState = localStateHistory.Get(serverTickNumber);

            //если прогнозируемое состояние совпадает с последним состоянием сервера,
            //прогнозируемое состояние сервера нужно применить к прогнозируемому игроку
            if (gameStateComparer.IsSame(predictedState, serverGameState, playerId))
            {
                GameState tempState = new GameState();
                tempState.Copy(serverGameState);
                gameStateCopier.CopyPlayerEntities(currentState, tempState, playerId);
                // заменить прогнозируемое состояние правильным состоянием сервера
                return localStateHistory.Put(tempState); 
            }
            else
            {
                //если предсказанное состояние игрока не совпадает с настоящим,
                //то пересимулировать положение игрока по историии ввода 
            
                //заменить предсказанное состояние на настоящее
                GameState lastGameState = localStateHistory.Put(serverGameState);
            
                for (int i = serverTickNumber; i < currentTick; i++) 
                {
                    //todo как это делать?
                    //пересоздать все неправильные состояния
                    lastGameState = prediction.Predict(lastGameState);
                }
            
                return lastGameState;
            }
        }
    }
}