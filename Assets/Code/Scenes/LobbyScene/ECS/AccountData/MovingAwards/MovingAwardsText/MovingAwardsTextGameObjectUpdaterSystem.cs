using System;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.MovingAwardsText
{
    /// <summary>
    /// Применяет изменяет прозрачноть и позицию текста в соответствии с данными компонентов.
    /// </summary>
    public class MovingAwardsTextGameObjectUpdaterSystem : IExecuteSystem
    {
        private readonly IGroup<LobbyUiEntity> awardsTextGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(MovingAwardsTextGameObjectUpdaterSystem));
        
        public MovingAwardsTextGameObjectUpdaterSystem(Contexts contexts)
        {
            awardsTextGroup = contexts.lobbyUi.GetGroup(LobbyUiMatcher
                .AllOf(LobbyUiMatcher.View, LobbyUiMatcher.Text, LobbyUiMatcher.AwardText,
                    LobbyUiMatcher.Position, LobbyUiMatcher.Alpha));
        }
        
        public void Execute()
        {
            var awardTextEntities = awardsTextGroup.GetEntities();
            for (int i = 0; i < awardTextEntities.Length; i++)
            {
                try
                {
                    LobbyUiEntity awardText = awardTextEntities[i];
                    awardText.view.gameObject.transform.position = awardText.position.value;
                    Color currentColor = awardText.text.Text.color;

                    float newAlpha = awardText.alpha.alpha;
                    awardText.text.Text.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
                }
                catch (Exception e)
                {
                    log.Error(e.Message+" "+e.StackTrace);
                }
            }
        }
    }
}