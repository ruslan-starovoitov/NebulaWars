using System;
using System.Collections.Generic;
using Entitas.Unity;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    public class ProjectileSpawner:IConcreteSpawner
    {
        private readonly int projectileLayerNumber;
        private readonly IPrefabStorage prefabStorage;
        private readonly PhysicsSpawner physicsSpawner;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipSpawner));

        public ProjectileSpawner(IPrefabStorage prefabStorage, PhysicsSpawner physicsSpawner)
        {
            this.prefabStorage = prefabStorage;
            this.physicsSpawner = physicsSpawner;
            projectileLayerNumber = LayerMask.NameToLayer("Projectiles");
            if (projectileLayerNumber == -1)
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
            GameObject go = physicsSpawner.SpawnProjectile(prefab, spawnPosition, spawnRotation);
            go.layer = projectileLayerNumber;
            entity.AddTransform(go.transform);
            Rigidbody rigidbody = go.GetComponent<Rigidbody>();
            entity.AddRigidbody(rigidbody);
            go.Link(entity);
            entity.AddView(go);
                
            if (entity.hasParentWarship)
            {
                var projectileCollider = go.GetComponent<Collider>();
                Collider[] warshipColliders = entity.parentWarship.entity.warshipColliders.colliders;
            
            
                Vector3 parentVelocity = entity.parentWarship.entity.rigidbody.value.velocity;
                rigidbody.velocity = parentVelocity;
                physicsSpawner.Ignore(new[]{projectileCollider}, warshipColliders );
            }
        }

        public List<ViewTypeEnum> GetWhatCanSpawn()
        {
            return new List<ViewTypeEnum>()
            {
                ViewTypeEnum.DefaultShoot
            };
        }
    }
}