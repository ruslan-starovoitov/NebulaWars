using System.Collections.Generic;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    public class HealthTextUpdatingSystem : ReactiveSystem<ServerGameEntity>
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthTextUpdatingSystem));
        
        public HealthTextUpdatingSystem(Contexts contexts)
            :base(contexts.serverGame)
        {
        }

        protected override ICollector<ServerGameEntity> GetTrigger(IContext<ServerGameEntity> context)
        {
            return context.CreateCollector(ServerGameMatcher.Health.Added(),
                ServerGameMatcher.MaxHealth.Added());
        }

        protected override bool Filter(ServerGameEntity entity)
        {
            return entity.hasHealth && entity.hasMaxHealth && entity.hasHealthBarParent;
        }

        protected override void Execute(List<ServerGameEntity> entities)
        {
            foreach (var entity in entities)
            {
                int health = entity.health.value;
                int maxHealth = entity.maxHealth.value;
                string value = $"{health}/{maxHealth}";
                entity.healthBarParent.healthBarEntity.healthBar.healthPoints.text = value;
                // log.Debug("Изменение хп "+value);
            }
        }
    }
}