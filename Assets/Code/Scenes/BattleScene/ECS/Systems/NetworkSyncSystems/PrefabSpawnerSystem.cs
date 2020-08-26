using System.Collections.Generic;
using Code.Common.Storages;
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
                ViewTypeEnum viewType = entity.viewType.value;
                GameObject prefab = prefabsStorage.GetPrefab(viewType);

                Vector3 position = entity.spawnTransform.position;
                Quaternion rotation = entity.spawnTransform.rotation;
                GameObject go = physicsSpawner.Spawn(prefab, position, rotation);
                entity.AddView(go);
                entity.AddTransform(go.transform);
                go.Link(entity);
                    
                //todo перенести
                if (entity.id.value == PlayerIdStorage.PlayerEntityId)
                {
                    ShootingPointsHelper shootingPointsHelper = new ShootingPointsHelper();
                    shootingPointsHelper.AddShootingPoints(go, entity);

                    Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                    entity.AddRigidbody(rigidbody);
                }
            }
        }
    }
}