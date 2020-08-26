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
            return context.CreateCollector(ServerGameMatcher.HealthPoints.Added(),
                ServerGameMatcher.MaxHealthPoints.Added());
        }

        protected override bool Filter(ServerGameEntity entity)
        {
            return entity.hasHealthPoints && entity.hasMaxHealthPoints && entity.hasHealthBarParent;
        }

        protected override void Execute(List<ServerGameEntity> entities)
        {
            foreach (var entity in entities)
            {
                string health = entity.healthPoints.value.ToString();
                string maxHealth = entity.maxHealthPoints.value.ToString();
                string value = $"{health}/{maxHealth}";
                entity.healthBarParent.healthBarEntity.healthBar.healthPoints.text = value;
                // log.Debug("Изменение хп "+value);
            }
        }
    }
}