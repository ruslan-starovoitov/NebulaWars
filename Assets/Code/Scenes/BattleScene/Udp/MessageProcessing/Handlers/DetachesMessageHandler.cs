using Code.Scenes.BattleScene.Udp.MessageProcessing.Synchronizers;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class DetachesMessageHandler : MessageHandler<DetachesMessage>
    {
        private readonly ParentsNetworkSynchronizer _synchronizer = ParentsNetworkSynchronizer.Instance;

        protected override void Handle(in DetachesMessage message, uint messageId, bool needResponse)
        {
            _synchronizer.HandleSet(messageId, message.DetachedIds);
        }
    }
}