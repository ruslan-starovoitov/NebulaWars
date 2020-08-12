using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class PlayerInfoMessageHandler : MessageHandler<PlayerInfoMessage>
    {
        private readonly IPlayersStorage playersStorage;

        public PlayerInfoMessageHandler(IPlayersStorage playersStorage)
        {
            this.playersStorage = playersStorage;
        }
        
        protected override void Handle(in PlayerInfoMessage message, uint messageId, bool needResponse)
        {
            PlayerIdStorage.PlayerEntityId = message.EntityIds[PlayerIdStorage.AccountId];
            playersStorage.SetNewPlayers(message.EntityIds);
        }
    }
}