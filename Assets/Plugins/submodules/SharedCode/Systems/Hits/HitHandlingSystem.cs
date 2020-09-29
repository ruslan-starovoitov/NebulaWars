using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Plugins.submodules.SharedCode.Systems.Hits
{
    /// <summary>
    /// Отнимает здоровье у цели и уничтожает снаряд.
    /// </summary>
    public class HitHandlingSystem : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<ServerGameEntity> hits;
        private readonly WarshipHitHandler warshipHitHandler;
        private readonly AsteroidHitHandler asteroidHitHandler;
        private readonly ILog log = LogManager.CreateLogger(typeof(HitHandlingSystem));

        public HitHandlingSystem(Contexts contexts)
        {
            hits = contexts.serverGame.GetGroup(ServerGameMatcher.Hit);
            warshipHitHandler = new WarshipHitHandler();
            asteroidHitHandler = new AsteroidHitHandler();
        }

        public void Execute()
        {
            foreach (var entity in hits)
            {
                ServerGameEntity targetEntity = entity.hit.targetEntity;
                ServerGameEntity projectileEntity = entity.hit.projectileEntity;

                if (!targetEntity.hasHealthPoints)
                {
                    string message = $"Попадание по объекту без hp id = {targetEntity.id.value}. " +
                                     $"Название объекта {targetEntity.view.gameObject.name}";
                    
                    log.Debug(message);
                    continue;
                }

                if (targetEntity.hasPlayer)
                {
                    warshipHitHandler.Hit(projectileEntity, targetEntity);
                }
                else
                {
                    asteroidHitHandler.Hit(projectileEntity, targetEntity);
                }
            }
        }

        public void Cleanup()
        {
            ServerGameEntity[] entities = hits.GetEntities();
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].Destroy();
            }
        }
    }
}