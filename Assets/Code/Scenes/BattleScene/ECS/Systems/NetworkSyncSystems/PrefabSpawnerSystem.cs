using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// При создании сущности с viewTypeId создаёт 3d модель из префаба
    /// </summary>
    public class PrefabSpawnerSystem:ReactiveSystem<GameEntity>
    {
        private readonly ViewTypePathStorage viewTypeStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PrefabSpawnerSystem));

        public PrefabSpawnerSystem(Contexts contexts) 
            : base(contexts.game)
        {
            viewTypeStorage = new ViewTypePathStorage();
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.ViewType.Added());
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasViewType && entity.hasTransform;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                var viewType = entity.viewType.id;
                string path = viewTypeStorage.GetPath(viewType);
                GameObject prefab = Resources.Load<GameObject>(path);
                var go = Object.Instantiate(prefab, entity.transform.position, Quaternion.identity);
                entity.AddView(go);
                go.Link(entity);
            }
        }
    }
}