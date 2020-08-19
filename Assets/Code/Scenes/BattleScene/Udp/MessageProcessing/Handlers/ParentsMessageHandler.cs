using Code.Scenes.BattleScene.Udp.MessageProcessing.Synchronizers;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class ParentsMessageHandler : MessageHandler<ParentsMessage>
    {
        private readonly ParentsNetworkSynchronizer _synchronizer = ParentsNetworkSynchronizer.Instance;

        protected override void Handle(in ParentsMessage message, uint messageId, bool needResponse)
        {
            _synchronizer.HandleDictionary(messageId, message.ParentInfo);
        }
    }
}