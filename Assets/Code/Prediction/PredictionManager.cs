namespace Code.Prediction
{
    public class PredictionManager
    {
        private readonly Predictor predictor;
        private readonly GameStateCopier gameStateCopier;
        private readonly IGameStateHistory gameStateHistory;
        private readonly GameStateComparer gameStateComparer;

        public PredictionManager(Predictor predictor, GameStateCopier gameStateCopier,
            IGameStateHistory gameStateHistory, GameStateComparer gameStateComparer)
        {
            this.predictor = predictor;
            this.gameStateCopier = gameStateCopier;
            this.gameStateHistory = gameStateHistory;
            this.gameStateComparer = gameStateComparer;
        }
        
        public GameState Reconcile(int currentTick, GameState serverGameState, GameState currentState, 
            ushort playerId)
        {
            int serverTickNumber =  serverGameState.tickNumber;
            GameState predictedState = gameStateHistory.Get(serverTickNumber);

            //если прогнозируемое состояние совпадает с последним состоянием сервера,
            //прогнозируемое состояние сервера нужно применить к прогнозируемому игроку
            if (gameStateComparer.IsSame(predictedState, serverGameState, playerId))
            {
                GameState tempState = new GameState();
                tempState.Copy(serverGameState);
                gameStateCopier.CopyPlayerEntities(currentState, tempState, playerId);
                // заменить неплохо предсказанное состояние точным состоянием из сервера
                return gameStateHistory.PutCorrectGameState(tempState); 
            }
            else
            {
                //если предсказанное состояние игрока не совпадает с настоящим,
                //то пересимулировать положение игрока по историии ввода 
            
                //заменить предсказанное состояние на настоящее
                GameState lastGameState = gameStateHistory.PutCorrectGameState(serverGameState);
            
                for (int i = serverTickNumber; i < currentTick; i++) 
                {
                    //todo как это делать?
                    //пересоздать все неправильные состояния
                    lastGameState = predictor.Predict(lastGameState);
                }
            
                return lastGameState;
            }
        }
    }
}