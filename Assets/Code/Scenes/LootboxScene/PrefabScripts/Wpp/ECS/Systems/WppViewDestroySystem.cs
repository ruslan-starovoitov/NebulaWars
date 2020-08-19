using System;
using System.Linq;
using Code.Common;
using Entitas;
using Entitas.Unity;
using Plugins.submodules.SharedCode.Logger;
using Object = UnityEngine.Object;

namespace Code.Scenes.LootboxScene.PrefabScripts.Wpp.ECS.Systems
{
    /// <summary>
    /// Удаляет объекты наград, когда они приближаются к месту назначения
    /// </summary>
    public class WppViewDestroySystem : IExecuteSystem
    {
        private readonly WppAccrualContext context;
        private readonly UiSoundsManager uiSoundsManager;
        private readonly IGroup<WppAccrualEntity> movingAwardsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(WppViewDestroySystem));

        public WppViewDestroySystem(Contexts contexts)
        {
            uiSoundsManager = UiSoundsManager.Instance();
            context = contexts.wppAccrual;
            movingAwardsGroup = context.GetGroup(WppAccrualMatcher.MovingIcon);
        }
        
        public void Execute()
        {
            DateTime now = DateTime.UtcNow;
            var awards = movingAwardsGroup.GetEntities();
            for (int index = 0; index < awards.Length; index++)
            {
                var entity = awards[index];
                DateTime arrivalTime = entity.movingIcon.iconTrajectory.controlPoints.Last().arrivalTime;
                if (arrivalTime <= now)
                {
                    // int increment = entity.movingIcon.increment;
                    Object.Destroy(entity.view.gameObject);
                    entity.Destroy();
                    uiSoundsManager.PlayWarshipPowerPointsAccrual();
                    //обновить значение шкалы
                    int oldValue = context.warshipPowerPoints.value;
                    context.ReplaceWarshipPowerPoints(oldValue + 1, context.warshipPowerPoints.maxValueForLevel);
                }
            }
        }
    }
}