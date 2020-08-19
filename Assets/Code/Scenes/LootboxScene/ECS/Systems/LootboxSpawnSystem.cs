using System;
using System.Collections.Generic;
using Code.Scenes.LootboxScene.PrefabScripts;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Scenes.LootboxScene.ECS.Systems
{
    public class LootboxSpawnSystem : ReactiveSystem<LootboxEntity>
    {
        private readonly Transform contentParent;
        private readonly GameObject lootboxPrefab;
        private readonly IContext<LootboxEntity> lootboxContext;
        private readonly ILog log = LogManager.CreateLogger(typeof(LootboxSpawnSystem));
        
        public LootboxSpawnSystem(IContext<LootboxEntity> lootboxContext, GameObject lootboxPrefab, Transform contentParent) 
            : base(lootboxContext)
        {
            this.lootboxContext = lootboxContext;
            this.lootboxPrefab = lootboxPrefab;
            this.contentParent = contentParent;

            if (lootboxPrefab == null)
            {
                throw new NullReferenceException();
            }
        }

        protected override ICollector<LootboxEntity> GetTrigger(IContext<LootboxEntity> context)
        {
            return context.CreateCollector(LootboxMatcher.ShowLootbox);
        }

        protected override bool Filter(LootboxEntity entity)
        {
            return entity.isShowLootbox;
        }

        protected override void Execute(List<LootboxEntity> entities)
        {
            GameObject go = Object.Instantiate(lootboxPrefab, contentParent);
            var lootboxOpeningController = go.GetComponent<LootboxOpeningController>();
            lootboxOpeningController.InitClose();
            lootboxContext.CreateEntity().AddNeedToOpenLootbox(lootboxOpeningController);
        }
    }
}