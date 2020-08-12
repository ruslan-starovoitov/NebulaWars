using System.Collections.Generic;
using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public interface ITransformStorage
    {
        void SetNewTransforms(uint messageId, Dictionary<ushort, ViewTransform> entitiesInfo);
    }
}