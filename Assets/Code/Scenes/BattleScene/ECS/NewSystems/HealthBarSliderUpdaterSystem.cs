using System;
using Code.Common.Logger;
using Code.Scenes.BattleScene.Udp.Connection;
using Entitas;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Обновляет слайдер и текст на полоске жизни.
    /// </summary>
    public class HealthBarSliderUpdaterSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> healthBarParentsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarSliderUpdaterSystem));

        public HealthBarSliderUpdaterSystem(Contexts contexts)
        {
            healthBarParentsGroup = contexts.game.GetGroup(GameMatcher.HealthBarParent);
        }

        public void Execute()
        {
            foreach (var entity in healthBarParentsGroup)
            {
                if (!entity.hasHealth)
                {
                    log.Error($"если есть HealthBarParent то обязательно должен быть Health");
                    continue;
                }
                
                if (!entity.hasMaxHealth)
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

                healthBarEntity.healthBar.slider.value = entity.health.value / entity.maxHealth.value;
                healthBarEntity.healthBar.healthPoints.text = ((int)entity.health.value).ToString();
            }
        }
    }
}