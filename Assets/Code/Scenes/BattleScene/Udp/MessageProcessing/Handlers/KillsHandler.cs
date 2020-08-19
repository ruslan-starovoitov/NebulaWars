using Code.Scenes.BattleScene.ECS.Systems;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class KillsHandler : MessageHandler<KillMessage>
    {
        protected override void Handle(in KillMessage message, uint messageId, bool needResponse)
        {
            KillsIndicatorSystem.AddNewKillInfo(message);
        }
    }
}