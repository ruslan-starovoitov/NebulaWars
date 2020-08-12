using System.Collections.Generic;
using System.Linq;
using Code.Common.Logger;
using Code.Common.Storages;
using Code.Scenes.BattleScene.Experimental;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public class UpdatePlayersSystem : IExecuteSystem, ITearDownSystem, IPlayersStorage
    {
        private readonly GameContext gameContext;
        private Dictionary<int, ushort> entityIds; //accountId, entityId 
        private readonly object lockObj = new object();
        private readonly Dictionary<int, BattleRoyalePlayerModel> playerInfos; //accountId, account
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdatePlayersSystem));
        
        public UpdatePlayersSystem(Contexts contexts, BattleRoyaleClientMatchModel matchModel)
        {
            gameContext = contexts.game;
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
                
                log.Debug("Обработка новых игроков "+entityIds.Count);
                // чтобы можно было удалять внутри foreach
                var currentEntityIds = new Dictionary<int, ushort>(entityIds); 

                foreach (var pair in currentEntityIds)
                {
                    var entity = gameContext.GetEntityWithId(pair.Value);

                    if (entity != null)
                    {
                        var accountId = pair.Key;
                        if (entity.hasPlayer)
                        {
                            log.Error("hasPlayer "+entity.id.value);
                            entity.RemovePlayer();
                        }
                        entity.AddPlayer(accountId, playerInfos[accountId].Nickname);
                        entityIds.Remove(accountId);
                    }
                }
            }
        }
    }
}