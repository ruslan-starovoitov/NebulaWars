using System;
using System.Collections.Generic;
using Plugins.submodules.SharedCode.Physics;
using Plugins.submodules.SharedCode.Systems.Spawn;

namespace Plugins.submodules.SharedCode.Systems
{
    public class SpawnManager:ISpawner
    {
        private readonly Dictionary<ViewTypeEnum, ISpawner> dictionary = new Dictionary<ViewTypeEnum, ISpawner>();

        public SpawnManager(IPrefabStorage prefabStorage, PhysicsSpawner physicsSpawner)
        {
            List<IConcreteSpawner> spawners = new List<IConcreteSpawner>()
            {
                new WarshipSpawner(prefabStorage, physicsSpawner),
                new AsteroidSpawner(prefabStorage, physicsSpawner),
                new ProjectileSpawner(prefabStorage, physicsSpawner)
            };
            
            foreach (IConcreteSpawner spawner in spawners)
            {
                List<ViewTypeEnum> list = spawner.GetWhatCanSpawn();
                foreach (var viewTypeEnum in list)
                {
                    dictionary.Add(viewTypeEnum, spawner);
                }
            }
        }
        public void Spawn(ServerGameEntity gameEntity)
        {
            ViewTypeEnum viewTypeEnum = gameEntity.viewType.value;
            if (dictionary.TryGetValue(viewTypeEnum, out ISpawner spawner))
            {
                spawner.Spawn(gameEntity);
            }
            else
            {
                throw new Exception($"Спавн сущности с таким viewType невозможен. viewTypeEnum={viewTypeEnum}");
            }
        }
    }
}