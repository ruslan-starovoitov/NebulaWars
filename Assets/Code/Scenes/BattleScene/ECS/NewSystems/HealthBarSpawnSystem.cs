using Code.Scenes.BattleScene.Scripts;
using Entitas;
using Entitas.Unity;
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
        private readonly HealthBarHeightStorage test;
        private readonly IGroup<GameEntity> needHealthBar;
        private readonly HealthBarStorage healthBarStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarSpawnSystem));

        public HealthBarSpawnSystem(Contexts contexts, HealthBarStorage healthBarStorage)
        {
            gameContext = contexts.game;
            this.healthBarStorage = healthBarStorage;
            needHealthBar = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.NeedHealthBar));
            test = new HealthBarHeightStorage();
        }
        
        public void Execute()
        {
            GameEntity[] entities = needHealthBar.GetEntities();
            for (var index = 0; index < entities.Length; index++)
            {
                GameEntity entity = entities[index];
                if (!entity.hasView)
                {
                    log.Error("Если есть NeedHealthBar, то обязательно должен быть view");
                    continue;
                }

                //Создать полоску
                GameEntity healthBarEntity = gameContext.CreateEntity();
                GameObject prefab = healthBarStorage.GetPrefab();
                GameObject go = Object.Instantiate(prefab);
                go.Link(entity);
                go.transform.position = new Vector3(0,test.GetHeight(entity.viewType.id));
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
                
                
                healthBarEntity.AddView(go);
                healthBarEntity.AddViewTransform(go.transform);
                healthBarEntity.AddHealthBar(slider, username, healthPoints, entity);

                if (entity.hasHealthBarParent)
                {
                    log.Error("У этой сущности не должно быть этого компонета.");
                    continue;
                }
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