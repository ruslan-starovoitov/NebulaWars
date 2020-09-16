using Code.Scenes.BattleScene.ECS.Systems;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing
{
    public class KillMessageHandler : MessageHandler<KillMessage>
    {
        private readonly IKillMessageStorage killMessageStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(KillMessageHandler));

        public KillMessageHandler(IKillMessageStorage killMessageStorage)
        {
            this.killMessageStorage = killMessageStorage;
        }
        
        protected override void Handle(in KillMessage message, uint messageId, bool needResponse)
        {
            killMessageStorage.AddKillModel(message);
        }
    }
}