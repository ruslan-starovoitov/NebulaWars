using System.Collections.Generic;
using Code.Common.Logger;
using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// При создании сущности с viewTypeId создаёт 3d модель из префаба
    /// </summary>
    public class PrefabSpawnerSystem:ReactiveSystem<GameEntity>
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(PrefabSpawnerSystem));
        
        public PrefabSpawnerSystem(Contexts contexts) 
            : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.ViewType.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasViewType && entity.hasTransform;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                log.Debug("Создание новой 3d модели");
                GameObject prefab = Resources.Load<GameObject>("Prefabs/3dWarships/StarSparrow1");
                var go = Object.Instantiate(prefab, entity.transform.position, Quaternion.identity);
                entity.AddView(go);
            }
        }
    }
}