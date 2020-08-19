using System;
using Code.Scenes.LobbyScene.ECS.Extensions;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.Images
{
    /// <summary>
    /// Меняет позиции GO движущихся наград.
    /// </summary>
    public class MovingIconsUpdaterSystem:IExecuteSystem
    {
        private readonly RectTransform upperObject;
        private readonly IGroup<LobbyUiEntity> movingAwardsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(MovingIconDataUpdaterSystem));

        public MovingIconsUpdaterSystem(Contexts contexts, RectTransform upperObject)
        {
            this.upperObject = upperObject;
            var contextsLobbyUi = contexts.lobbyUi;
            movingAwardsGroup = contextsLobbyUi.GetGroup(LobbyUiMatcher
                .AllOf(LobbyUiMatcher.MovingIcon, LobbyUiMatcher.View, LobbyUiMatcher.Position));
        }
        
        public void Execute()
        {
            foreach (LobbyUiEntity entity in movingAwardsGroup)
            {
                entity.view.gameObject.transform.position = entity.position.value;
                entity.view.gameObject.transform.localScale = entity.scale.scale;
            
                Color tmpColor = entity.image.image.color;
                tmpColor.a = entity.alpha.alpha;
                entity.image.image.color = tmpColor;
                
                if (entity.movingIcon.IsRaiseUpNeeded())
                {
                    entity.view.gameObject.transform.SetParent(upperObject, false);
                    entity.movingIcon.TurnOffRaiseUp();
                }
            }
        }
    }
}