using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class HidesMessageHandler : MessageHandler<HidesMessage>
    {
        protected override void Handle(in HidesMessage message, uint messageId, bool needResponse)
        {
            UpdateHidingSystem.SetNewHides(messageId, message.HiddenIds);
        }
    }
}