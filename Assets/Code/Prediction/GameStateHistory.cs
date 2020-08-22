using System.Collections.Concurrent;

namespace Code.Prediction
{
    public class GameStateHistory
    {
        private readonly ConcurrentDictionary<int, GameState> history = new ConcurrentDictionary<int, GameState>();
        public GameState Get(int serverTickNumber)
        {
            return history[serverTickNumber];
        }

        /// <summary>
        /// Заменяет предсказаннео игровое состояние на настоящее с сервера
        /// </summary>
        public GameState Put(GameState tempState)
        {
            int tickNumber = tempState.tickNumber;
            if (history.ContainsKey(tickNumber))
            {
                history[tickNumber] = tempState;
            }
            else
            {
                history.TryAdd(tickNumber, tempState);
            }

            return tempState;
        }
    }
}