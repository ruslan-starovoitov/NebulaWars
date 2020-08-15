using Code.Common.Logger;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

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
            // log.Debug($"Информация о игроках messageId "+messageId);
            ushort playerEntityId = message.EntityIds[PlayerIdStorage.AccountId];
            // log.Debug($"playerTmpId "+playerTmpId);
            // log.Debug($"PlayerIdStorage.AccountId "+PlayerIdStorage.AccountId);
            PlayerIdStorage.PlayerEntityId = playerEntityId;
            playersStorage.SetNewPlayers(message.EntityIds);
        }
    }
}