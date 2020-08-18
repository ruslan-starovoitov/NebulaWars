using System.Collections.Generic;
using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public interface ITransformStorage
    {
        void SetNewTransforms(in PositionsMessage message);
    }
}