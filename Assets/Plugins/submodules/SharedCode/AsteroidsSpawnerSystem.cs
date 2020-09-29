using System.Collections.Generic;
using Entitas.Unity;
using Plugins.submodules.SharedCode.Physics;
using Plugins.submodules.SharedCode.Systems.Spawn;
using UnityEngine;

namespace Plugins.submodules.SharedCode
{
    public class AsteroidSpawner:IConcreteSpawner
    {
        private readonly IPrefabStorage prefabStorage;
        private readonly PhysicsSpawner physicsSpawner;

        public AsteroidSpawner(IPrefabStorage prefabStorage, PhysicsSpawner physicsSpawner)
        {
            this.prefabStorage = prefabStorage;
            this.physicsSpawner = physicsSpawner;
        }

        public void Spawn(ServerGameEntity entity)
        {
            ViewTypeEnum viewType = entity.viewType.value;
            Vector3 spawnPosition = entity.spawnTransform.position;
            Quaternion spawnRotation = entity.spawnTransform.rotation;
            GameObject prefab = prefabStorage.GetPrefab(viewType);
            GameObject go = physicsSpawner.Spawn(prefab, spawnPosition, spawnRotation);
            go.Link(entity);
            entity.AddTransform(go.transform);
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            entity.AddRigidbody(rigidbody);
            entity.AddView(go);

            //todo не забыть убрать компонент
            // entity.RemoveSpawnTransform();
        }

        public List<ViewTypeEnum> GetWhatCanSpawn()
        {
            return new List<ViewTypeEnum>()
            {
                ViewTypeEnum.AsteroidLavaBlue,
                ViewTypeEnum.AsteroidLavaRed,
                ViewTypeEnum.MineBlue,
                ViewTypeEnum.MineRed
            };
        }
    }
}