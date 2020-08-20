using System.Collections.Generic;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Prediction
{
    public class GameStateBuffer:ITransformStorage
    {
        private readonly Dictionary<int, GameState> buffer = new Dictionary<int, GameState>();
        public void SetNewTransforms(in PositionsMessage message)
        {
            throw new System.NotImplementedException();
        }

        public bool IsReady()
        {
            //todo начинать симуляцию после того, как буффер заполнится на три кадра
            return buffer.Count >= 3;
        }
    }
}