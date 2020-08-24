using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using Plugins.submodules.SharedCode.Systems.Spawn;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// При создании сущности с ViewTypeEnum создаёт 3d модель из префаба
    /// </summary>
    public class PrefabSpawnerSystem:ReactiveSystem<GameEntity>
    {
        private readonly PrefabsStorage prefabsStorage;
        private readonly PhysicsSpawner physicsSpawner;
        private readonly ILog log = LogManager.CreateLogger(typeof(PrefabSpawnerSystem));

        public PrefabSpawnerSystem(Contexts contexts, PrefabsStorage prefabsStorage, PhysicsSpawner physicsSpawner) 
            : base(contexts.game)
        {
            this.prefabsStorage = prefabsStorage;
            this.physicsSpawner = physicsSpawner;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.ViewType.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasViewType && entity.hasTransform2D;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                var viewType = entity.viewType.id;
                GameObject prefab = prefabsStorage.GetPrefab(viewType);
                GameObject go = physicsSpawner.Spawn(prefab, entity.transform2D.position, Quaternion.identity);
                entity.AddView(go);
                entity.AddViewTransform(go.transform);
                go.Link(entity);
            }
        }
    }
}