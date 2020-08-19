using Code.Scenes.BattleScene.ECS.Systems;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Cooldown;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class CooldownsHandler : MessageHandler<CooldownsMessage>
    {
        protected override void Handle(in CooldownsMessage message, uint messageId, bool needResponse)
        {
            CooldownsUpdaterSystem.SetCooldowns(message.WeaponsCooldowns);
            AbilityUpdaterSystem.SetCurrentCooldown(message.AbilityCooldown);
        }
    }
}