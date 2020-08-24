using System;
using Code.Scenes.LobbyScene.ECS.Extensions;
using Entitas;
using Plugins.submodules.EntitasCore.Entitas.Components;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.Images
{
    /// <summary>
    /// Меняет позицию компонента движущейся награды.
    /// </summary>
    public class MovingIconDataUpdaterSystem:IExecuteSystem
    {
        private readonly IGroup<LobbyUiEntity> movingAwardsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(MovingIconDataUpdaterSystem));

        public MovingIconDataUpdaterSystem(Contexts contexts)
        {
            LobbyUiContext contextsLobbyUi = contexts.lobbyUi;
            movingAwardsGroup = contextsLobbyUi.GetGroup(LobbyUiMatcher.AllOf(
                LobbyUiMatcher.MovingIcon, LobbyUiMatcher.Alpha, LobbyUiMatcher.Position,
                LobbyUiMatcher.Scale));
        }
        
        public void Execute()
        {
            DateTime now = DateTime.Now;
            foreach (LobbyUiEntity award in movingAwardsGroup)
            {
                MovingIconComponent movingAward = award.movingIcon;
                DateTime currentTargetArrivalTime = movingAward.GetCurrentTargetArrivalTime();

                //Если уже нужно переходить на новый отрезок пути 
                if (currentTargetArrivalTime <= now)
                {
                    //то стать в последнюю точку текущего отрезка
                    award.position.value = movingAward.GetCurrentTargetPoint(); 
                    
                    //если отрезки ещё есть, то перейти дальше
                    if (movingAward.iconTrajectory.currentControlPointIndex + 1 < movingAward.iconTrajectory.controlPoints.Count)
                    {
                        movingAward.iconTrajectory.currentControlPointIndex++;
                    }
                }
                //иначе сдвинуть в зависимости от времени
                else
                {
                    award.position.value = movingAward.CalculatePosition(now);
                    award.scale.scale = movingAward.CalculateScale(now);
                    award.alpha.alpha = movingAward.CalculateAlpha(now);
                }
            }
        }
    }
}