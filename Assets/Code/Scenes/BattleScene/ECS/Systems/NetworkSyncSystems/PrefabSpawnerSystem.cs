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
    public class PrefabSpawnerSystem:ReactiveSystem<ServerGameEntity>
    {
        private readonly PrefabsStorage prefabsStorage;
        private readonly PhysicsSpawner physicsSpawner;
        private readonly ILog log = LogManager.CreateLogger(typeof(PrefabSpawnerSystem));

        public PrefabSpawnerSystem(Contexts contexts, PrefabsStorage prefabsStorage, PhysicsSpawner physicsSpawner) 
            : base(contexts.serverGame)
        {
            this.prefabsStorage = prefabsStorage;
            this.physicsSpawner = physicsSpawner;
        }

        protected override ICollector<ServerGameEntity> GetTrigger(IContext<ServerGameEntity> context)
        {
            return context.CreateCollector(ServerGameMatcher.ViewType.Added());
        }

        protected override bool Filter(ServerGameEntity entity)
        {
            return entity.hasViewType && entity.hasSpawnTransform;
        }

        protected override void Execute(List<ServerGameEntity> entities)
        {
            foreach (var entity in entities)
            {
                var viewType = entity.viewType.value;
                GameObject prefab = prefabsStorage.GetPrefab(viewType);
                GameObject go = physicsSpawner.Spawn(prefab, entity.spawnTransform.position, 
                    entity.spawnTransform.rotation);
                entity.AddView(go);
                entity.AddTransform(go.transform);
                go.Link(entity);
            }
        }
    }
}