using System;
using System.Collections.Generic;
using System.Linq;
using Entitas;
using Plugins.submodules.EntitasCore.Entitas.Components;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.ECS.MatchSearch
{
    /// <summary>
    /// Обновляет текст в меню при добавлении компонента.
    /// </summary>
    public class MatchSearchMenuUpdaterSystem:ReactiveSystem<LobbyUiEntity>
    {
        private readonly LobbyUiContext lobbyUiContext;
        private readonly Text numberOfPlayersInQueueGameObject;
        private readonly Text numberOfPlayersInBattlesGameObject;

        public MatchSearchMenuUpdaterSystem(LobbyUiContext lobbyUiContext,  
            Text numberOfPlayersInQueueGameObject, Text numberOfPlayersInBattlesGameObject)
            :base(lobbyUiContext)
        {
            this.lobbyUiContext = lobbyUiContext;
            if (numberOfPlayersInQueueGameObject == null)
            {
                throw new Exception($"{nameof(numberOfPlayersInQueueGameObject)} was null");
            }

            if (numberOfPlayersInBattlesGameObject == null)
            {
                throw new Exception($"{nameof(numberOfPlayersInBattlesGameObject)} was null");
            }
            
            this.numberOfPlayersInQueueGameObject = numberOfPlayersInQueueGameObject;
            this.numberOfPlayersInBattlesGameObject = numberOfPlayersInBattlesGameObject;
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.MatchSearchDataForMenu.Added());
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasMatchSearchDataForMenu;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            MatchSearchDataForMenuComponent matchSearchDataForMenuComponent = entities.Last().matchSearchDataForMenu;
            
            if (lobbyUiContext.hasStartButtonClicked)
            {
                numberOfPlayersInQueueGameObject.text = "Players in queue: " +
                                                        matchSearchDataForMenuComponent.NumberOfPlayersInQueue;
                numberOfPlayersInBattlesGameObject.text = "Players in match: " + 
                                                          matchSearchDataForMenuComponent.numberOfPlayersInMatch;
            }
        }
    }
}