using System.Collections.Generic;
using System.Linq;
using Code.Common.Storages;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public class UpdatePlayersSystem : IExecuteSystem, ITearDownSystem, IPlayersStorage
    {
        private readonly ServerGameContext gameContext;
        private readonly object lockObj = new object();
        private Dictionary<int, ushort> entityIds; //accountId, entityId 
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdatePlayersSystem));
        private readonly Dictionary<int, BattleRoyalePlayerModel> playerInfos; //accountId, account
        
        public UpdatePlayersSystem(Contexts contexts, BattleRoyaleClientMatchModel matchModel)
        {
            gameContext = contexts.serverGame;
            playerInfos = matchModel.PlayerModels
                .ToDictionary(item => item.AccountId);
        }

        public void SetNewPlayers(Dictionary<int, ushort> newPlayers)
        {
            lock (lockObj)
            {
                entityIds = newPlayers;
            }
        }

        public void TearDown()
        {
            entityIds?.Clear();
        }

        public void Execute()
        {
            lock (lockObj)
            {
                if (entityIds == null)
                {
                    return;
                }
                
                if (entityIds.Count == 0)
                {
                    return;
                }
                
                log.Info("Обработка новых игроков "+entityIds.Count);
                var currentEntityIds = new Dictionary<int, ushort>(entityIds);
                foreach (var pair in currentEntityIds)
                {
                    int accountId = pair.Key;
                    ushort entityId = pair.Value;
                    ServerGameEntity entity = gameContext.GetEntityWithId(entityId);

                    //todo обновление id сущности игрока
                    int currentPlayerAccountId = PlayerIdStorage.AccountId;
                    if (accountId == currentPlayerAccountId)
                    {
                        PlayerIdStorage.PlayerEntityId = entityId;
                    }
                    
                    
                    if (entity != null)
                    {
                        entity.ReplaceNickname(playerInfos[accountId].Nickname);
                        entityIds.Remove(accountId);
                    }
                    else
                    {
                        log.Debug("Нет сущности с таким id");
                    }
                }
            }
        }
    }
}