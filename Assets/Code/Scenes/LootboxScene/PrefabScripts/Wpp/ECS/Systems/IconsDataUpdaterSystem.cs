using System;
using Code.Scenes.LobbyScene.ECS.Extensions;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LootboxScene.PrefabScripts.Wpp.ECS.Systems
{
    /// <summary>
    /// Меняет позицию компонента движущейся награды.
    /// </summary>
    public class IconsDataUpdaterSystem:IExecuteSystem
    {
        private readonly IGroup<WppAccrualEntity> movingIconsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(IconsDataUpdaterSystem));

        public IconsDataUpdaterSystem(Contexts contexts)
        {
            var context = contexts.wppAccrual;
            movingIconsGroup = context.GetGroup(WppAccrualMatcher.AllOf(
                WppAccrualMatcher.MovingIcon, WppAccrualMatcher.Alpha, WppAccrualMatcher.Position,
                WppAccrualMatcher.Scale));
        }
        
        public void Execute()
        {
            DateTime now = DateTime.UtcNow;
            foreach (var entity in movingIconsGroup)
            {
                var movingAward = entity.movingIcon;
                DateTime currentTargetArrivalTime = movingAward.GetCurrentTargetArrivalTime();

                //Если уже нужно переходить на новый отрезок пути 
                if (currentTargetArrivalTime <= now)
                {
                    //то стать в последнюю точку текущего отрезка
                    entity.position.value = movingAward.GetCurrentTargetPoint(); 
                    
                    //если отрезки ещё есть, то перейти дальше
                    if (movingAward.iconTrajectory.currentControlPointIndex + 1 < movingAward.iconTrajectory.controlPoints.Count)
                    {
                        movingAward.iconTrajectory.currentControlPointIndex++;
                    }
                }
                //иначе сдвинуть в зависимости от времени
                else
                {
                    entity.position.value = movingAward.CalculatePosition(now);
                    entity.scale.scale = movingAward.CalculateScale(now);
                    entity.alpha.alpha = movingAward.CalculateAlpha(now);
                }
            }
        }
    }
}