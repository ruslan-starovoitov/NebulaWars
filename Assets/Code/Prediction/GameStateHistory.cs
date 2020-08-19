using System;

namespace Code.Prediction
{
    public class GameStateHistory
    {
        public GameState Get(int tick)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Заменяет предсказаннео игровое состояние на настоящее с сервера
        /// </summary>
        /// <param name="tempState"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public GameState Put(GameState tempState)
        {
            throw new NotImplementedException();
        }
    }
}