using Code.Scenes.BattleScene.ECS.Systems;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Shield;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class MaxShieldPointsHandler : MessageHandler<MaxShieldPointsMessage>
    {
        protected override void Handle(in MaxShieldPointsMessage message, uint messageId, bool needResponse)
        {
            HealthAndShieldPointsUpdaterSystem.SetMaxShieldPoints(message.Value);
        }
    }
}