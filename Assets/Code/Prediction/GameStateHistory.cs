using System;
using System.Collections.Concurrent;

namespace Code.Prediction
{
    public interface IGameStateHistory
    {
        GameState Get(int serverTickNumber);
        void PutPredictedGameState(GameState predictedGameState);
        GameState PutCorrectGameState(GameState correctGameState);
    }
}