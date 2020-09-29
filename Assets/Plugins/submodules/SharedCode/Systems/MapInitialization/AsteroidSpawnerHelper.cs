using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.MapInitialization
{
    /// <summary>
    /// Создаёт сущность-команду создать автероид
    /// </summary>
    public class AsteroidSpawnerHelper
    {
        private readonly Contexts contexts;

        public AsteroidSpawnerHelper(Contexts contexts)
        {
            this.contexts = contexts;
        }
        
        public void CreateAsteroid(Vector3 position, ViewTypeEnum viewTypeEnum)
        {
            ServerGameEntity entity = contexts.serverGame.CreateEntity();
            entity.AddHealthPoints(500);
            entity.AddMaxHealthPoints(500);
            entity.AddViewType(viewTypeEnum);
            entity.AddSpawnTransform(position, Quaternion.identity);
        }
    }
}