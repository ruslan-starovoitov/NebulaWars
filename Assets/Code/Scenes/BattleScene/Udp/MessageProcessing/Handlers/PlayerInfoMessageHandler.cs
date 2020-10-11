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
            //todo опасность забыть обнулить playerEntityId
            if (PlayerIdStorage.PlayerEntityId == 0)
            {
                int playerAccountId = PlayerIdStorage.AccountId;
                if (message.entityIds.ContainsKey(playerAccountId))
                {
                    log.Debug("Установка playerEntityId = "+message.entityIds[playerAccountId]);
                    PlayerIdStorage.PlayerEntityId = message.entityIds[playerAccountId];
                }
                else
                {
                    log.Debug("В снимке нет данных про игрока.");
                }
            }
            
            playersStorage.SetNewPlayers(message.entityIds);
        }
    }
}