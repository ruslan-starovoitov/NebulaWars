using System.Collections.Generic;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Plugins.submodules.SharedCode.Systems
{
    public class PrefabSpawnerSystem:ReactiveSystem<ServerGameEntity>
    {
        private readonly SpawnManager spawnManager;
        private readonly ILog log = LogManager.CreateLogger(typeof(PrefabSpawnerSystem));

        public PrefabSpawnerSystem(Contexts contexts, SpawnManager spawnManager) 
            : base(contexts.serverGame)
        {
            this.spawnManager = spawnManager;
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
                spawnManager.Spawn(entity);
            }
        }
    }
}