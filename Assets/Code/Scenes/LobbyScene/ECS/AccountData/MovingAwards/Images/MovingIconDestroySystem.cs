using Code.Common;
using Entitas;
using System;
using System.Linq;
using Plugins.submodules.EntitasCore.Entitas.Components;
using Plugins.submodules.SharedCode.Logger;
using Object = UnityEngine.Object;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.Images
{
    /// <summary>
    /// Удаляет объекты наград, когда они приближаются к месту назначения
    /// </summary>
    public class MovingIconDestroySystem : IExecuteSystem
    {
        private readonly LobbyUiContext lobbyUiContext;
        private readonly UiSoundsManager uiSoundsManager;
        private readonly IGroup<LobbyUiEntity> movingAwardsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(MovingIconDataUpdaterSystem));

        public MovingIconDestroySystem(Contexts contexts)
        {
            uiSoundsManager = UiSoundsManager.Instance();
            lobbyUiContext = contexts.lobbyUi;
            movingAwardsGroup = lobbyUiContext.GetGroup(LobbyUiMatcher.MovingIcon);
        }
        
        public void Execute()
        {
            DateTime now = DateTime.Now;
            LobbyUiEntity[] awards = movingAwardsGroup.GetEntities();
            for (int index = 0; index < awards.Length; index++)
            {
                LobbyUiEntity movingAward = awards[index];
                DateTime arrivalTime = movingAward.movingIcon.iconTrajectory.controlPoints.Last().arrivalTime;
                if (arrivalTime <= now)
                {
                    int increment = movingAward.movingIcon.increment;
                    switch (movingAward.movingIcon.awardTypeEnum)
                    {
                        case AwardTypeEnum.SoftCurrency:
                            uiSoundsManager.PlaySoftAdding();
                            lobbyUiContext.ReplaceSoftCurrency(lobbyUiContext.softCurrency.value+increment);
                            break;
                        case AwardTypeEnum.AccountRating:
                            uiSoundsManager.PlayRatingAdding();
                            lobbyUiContext.ReplaceAccountRating(lobbyUiContext.accountRating.value+increment);
                            break;
                        case AwardTypeEnum.HardCurrency:
                            uiSoundsManager.PlayHardAdding();
                            lobbyUiContext.ReplaceHardCurrency(lobbyUiContext.hardCurrency.value+increment);
                            break;
                        case AwardTypeEnum.LootboxPoints:
                            uiSoundsManager.PlayPointsAdding();
                            lobbyUiContext.ReplacePointsForSmallLootbox(lobbyUiContext.pointsForSmallLootbox.value+increment);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    Object.Destroy(movingAward.view.gameObject);
                    movingAward.Destroy();
                }
            }
        }
    }
}