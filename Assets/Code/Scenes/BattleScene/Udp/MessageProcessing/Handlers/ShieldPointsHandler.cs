using Code.Scenes.BattleScene.ECS.Systems;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Shield;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class ShieldPointsHandler : MessageHandler<ShieldPointsMessage>
    {
        protected override void Handle(in ShieldPointsMessage message, uint messageId, bool needResponse)
        {
            HealthAndShieldPointsUpdaterSystem.SetShieldPoints(message.Value);
        }
    }
}