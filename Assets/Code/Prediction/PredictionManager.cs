namespace Code.Prediction
{
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
            uint playerId)
        {
            GameState serverState =  serverStateData.GameState;
            int serverTick = serverState.Time;
            
            
            GameState predictedState = localStateHistory.Get(serverTick);

            //if predicted state matches server last state use server predicted state with predicted player
            if (gameStateComparer.IsSame(predictedState, serverState, playerId))
            {
                GameState tempState = new GameState();
                tempState.Copy(serverState);
                gameStateCopier.CopyPlayerEntities(currentState, tempState, playerId);
                return localStateHistory.Put(tempState); // replace predicted state with correct server state
            }

            //if predicted state doesn't match server state, reapply local inputs to server state
            var last = localStateHistory.Put(serverState); // replace wrong predicted state with correct server state
            for (int i = serverTick; i < currentTick; i++) 
            {
                //todo как это делать?
                last = prediction.Predict(last); // resimulate all wrong states
            }
            return last;
        }
    }
}