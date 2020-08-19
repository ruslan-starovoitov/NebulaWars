using Code.Scenes.BattleScene.ECS.Systems;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Health;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class MaxHealthPointsHandler:MessageHandler<MaxHealthPointsMessage>
    {
        protected override void Handle(in MaxHealthPointsMessage message, uint messageId, bool needResponse)
        {
            HealthAndShieldPointsUpdaterSystem.SetMaxHealthPoints(message.Value);
        }
    }
}