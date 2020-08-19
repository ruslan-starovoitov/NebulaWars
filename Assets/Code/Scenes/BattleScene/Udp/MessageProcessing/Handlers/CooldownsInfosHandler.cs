using Code.Scenes.BattleScene.ECS.Systems;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Cooldown;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class CooldownsInfosHandler : MessageHandler<CooldownsInfosMessage>
    {
        protected override void Handle(in CooldownsInfosMessage message, uint messageId, bool needResponse)
        {
            CooldownsUpdaterSystem.SetWeaponsInfos(message.WeaponsInfos);
            AbilityUpdaterSystem.SetMaxCooldown(message.AbilityMaxCooldown);
        }
    }
}