using Code.Scenes.BattleScene.Scripts;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Добавляет полоску жизни сущностям у которых есть hp и объект
    /// </summary>
    public class HealthBarSpawnSystem:IExecuteSystem, ICleanupSystem
    {
        private readonly GameContext gameContext;
        private readonly IGroup<GameEntity> needHealthBar;
        private readonly HealthBarStorage healthBarStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarSpawnSystem));

        public HealthBarSpawnSystem(Contexts contexts, HealthBarStorage healthBarStorage)
        {
            gameContext = contexts.game;
            this.healthBarStorage = healthBarStorage;
            needHealthBar = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.NeedHealthBar));
        }
        
        public void Execute()
        {
            var test = needHealthBar.GetEntities();
            for (var index = 0; index < test.Length; index++)
            {
                var entity = test[index];

                if (!entity.hasView)
                {
                    log.Error("Если есть NeedHealthBar, то обязательно должен быть view");
                }
            
                
                //Создать полоску
                GameObject prefab = healthBarStorage.GetPrefab();
                Transform parent = entity.view.gameObject.transform;
                
                //вынести это
                GameObject go = Object.Instantiate(prefab, parent);
                Slider slider = go.transform.Find("Slider").GetComponent<Slider>();
                if (slider == null)
                {
                    log.Error("Не найден слайдер на полоске хп");
                    continue;
                }
                
                TextMeshProUGUI username = go.transform.Find("Text_Username").GetComponent<TextMeshProUGUI>();
                if (username == null)
                {
                    log.Error("Не найден text username на полоске хп");
                    continue;
                }
                
                TextMeshProUGUI healthPoints = go.transform.Find("Slider/Text_HealthPoints")
                    .GetComponent<TextMeshProUGUI>();
                if (healthPoints == null)
                {
                    log.Error("Не найден text healthPoints на полоске хп");
                    continue;
                }
                
                //вынести это

                var healthBarEntity = gameContext.CreateEntity();
                healthBarEntity.AddView(go);
                healthBarEntity.AddHealthBar(slider, username, healthPoints);
                
                entity.AddHealthBarParent(healthBarEntity);
            }
        }

        public void Cleanup()
        {
            var entities = needHealthBar.GetEntities();
            for (var index = 0; index < entities.Length; index++)
            {
                var entity = entities[index];
                entity.isNeedHealthBar = false;
            }
        }
    }
}