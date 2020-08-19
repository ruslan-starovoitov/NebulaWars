using System.Collections.Generic;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Prediction
{
    public class GameStateBuffer:ITransformStorage
    {
        //todo начинать симуляцию после того, как буффер заполнится на три кадра
        private Dictionary<int, GameState> buffer;
        public void SetNewTransforms(in PositionsMessage message)
        {
            throw new System.NotImplementedException();
        }

        public bool IsReady()
        {
            return buffer.Count >= 3;
        }
    }
}