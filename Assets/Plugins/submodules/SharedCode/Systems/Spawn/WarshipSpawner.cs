using System;
using System.Collections.Generic;
using Entitas.Unity;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    public class WarshipSpawner:IConcreteSpawner
    {
        private readonly int playersLayerNumber;
        private readonly IPrefabStorage prefabStorage;
        private readonly PhysicsSpawner physicsSpawner;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipSpawner));
        private readonly ShootingPointsHelper shootingPointsHelper = new ShootingPointsHelper();
        
        public WarshipSpawner(IPrefabStorage prefabStorage, PhysicsSpawner physicsSpawner)
        {
            this.prefabStorage = prefabStorage;
            this.physicsSpawner = physicsSpawner;
            playersLayerNumber = LayerMask.NameToLayer("Players");
            if (playersLayerNumber == -1)
            {
                throw new Exception("Не найден номер слоя.");
            }
        }
        
        public void Spawn(ServerGameEntity entity)
        {
            ViewTypeEnum viewType = entity.viewType.value;
            Vector3 spawnPosition = entity.spawnTransform.position;
            Quaternion spawnRotation = entity.spawnTransform.rotation;
            GameObject prefab = prefabStorage.GetPrefab(viewType);
            GameObject go = physicsSpawner.Spawn(prefab, spawnPosition, spawnRotation);
            go.layer = playersLayerNumber;
            go.Link(entity);
            entity.AddView(go);
            entity.AddTransform(go.transform);
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            entity.AddRigidbody(rigidbody);
            shootingPointsHelper.AddShootingPoints(go, entity);
            Collider[] colliders = go.GetComponents<Collider>();
            entity.AddWarshipColliders(colliders);
        }

        public List<ViewTypeEnum> GetWhatCanSpawn()
        {
            return new List<ViewTypeEnum>()
            {
                ViewTypeEnum.StarSparrow1
            };
        }
    }
}