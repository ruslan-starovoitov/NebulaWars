using System;

namespace Code.Prediction
{
    public class GameState
    {
        public int Time { get; set; }
        public object[] WorldState { get; set; }

        public void Copy(GameState serverState)
        {
            throw new NotImplementedException();
        }
    }
}