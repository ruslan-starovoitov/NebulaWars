using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class PlayerInfoMessageHandler : MessageHandler<PlayerInfoMessage>
    {
        private readonly IPlayersStorage playersStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerInfoMessageHandler));

        public PlayerInfoMessageHandler(IPlayersStorage playersStorage)
        {
            this.playersStorage = playersStorage;
        }
        
        protected override void Handle(in PlayerInfoMessage message, uint messageId, bool needResponse)
        {
            playersStorage.SetNewPlayers(message.entityIds);
        }
    }
}