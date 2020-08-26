using System.Globalization;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Обновляет слайдер и текст на полоске жизни.
    /// </summary>
    public class HealthBarSliderUpdaterSystem : IExecuteSystem
    {
        private readonly IGroup<ServerGameEntity> healthBarParentsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarSliderUpdaterSystem));

        public HealthBarSliderUpdaterSystem(Contexts contexts)
        {
            healthBarParentsGroup = contexts.serverGame.GetGroup(ServerGameMatcher.HealthBarParent);
        }

        public void Execute()
        {
            foreach (var entity in healthBarParentsGroup)
            {
                if (!entity.hasHealthPoints)
                {
                    log.Error($"если есть HealthBarParent то обязательно должен быть Health");
                    continue;
                }
                
                if (!entity.hasMaxHealthPoints)
                {
                    log.Error($"если есть HealthBarParent то обязательно должен быть MaxHealth");
                    continue;
                }

                var healthBarEntity = entity.healthBarParent.healthBarEntity;
                if (!healthBarEntity.hasHealthBar)
                {
                    log.Error($"если есть HealthBarParent то обязательно должен быть MaxHealth");
                    continue;
                }

                healthBarEntity.healthBar.slider.value = 1f* entity.healthPoints.value / entity.maxHealthPoints.value;
                healthBarEntity.healthBar.healthPoints.text = entity.healthPoints.value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}