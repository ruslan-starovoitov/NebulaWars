using System;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Object = UnityEngine.Object;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.MovingAwardsText
{
    /// <summary>
    /// Удаляет текст, когда он стал прозрачным.
    /// </summary>
    public class MovingAwardsTextDeleteSystem : IExecuteSystem
    {
        private readonly IGroup<LobbyUiEntity> awardsTextGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(MovingAwardsTextDeleteSystem));
        

        public MovingAwardsTextDeleteSystem(Contexts contexts)
        {
            awardsTextGroup = contexts.lobbyUi.GetGroup(LobbyUiMatcher
                .AllOf(LobbyUiMatcher.View, LobbyUiMatcher.Text, LobbyUiMatcher.AwardText));
        }
        
        public void Execute()
        {
            var awardTextEntities = awardsTextGroup.GetEntities();
            DateTime now = DateTime.Now;
            for (int i = 0; i < awardTextEntities.Length; i++)
            {
                var awardText = awardTextEntities[i];
                
                if (awardText.awardText.fadeTime < now)
                {
                    Object.Destroy(awardText.view.gameObject);
                    awardText.Destroy();
                }
            }
        }
    }
}