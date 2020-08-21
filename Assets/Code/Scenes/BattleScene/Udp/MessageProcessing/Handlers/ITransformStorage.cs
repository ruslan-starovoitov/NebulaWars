using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public interface ITransformStorage
    {
        void SetNewTransforms(TransformPackMessage message);
    }
}