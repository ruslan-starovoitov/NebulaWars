using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Experimental.SystemsOrderChecker;
using Code.Scenes.LootboxScene.Scripts;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LootboxScene.ECS.Systems
{
    /// <summary>
    /// 1) При нажатии на канвас проверяет, что лутбокс ещё не открыт.
    /// 2) Если это так, то открывает лутбокс и уничтожает сообщения клика на канвас.
    /// 3) Если анимация открытия лутбокса закончилась и не были показаны следующие призы,
    /// то будет создано сообщение клика на канвас.
    /// </summary>
    public class LootboxOpeningSystem : ReactiveSystem<LootboxEntity>, IRequireSystemsInvocationOrderChecker
    {
        private readonly Transform contentParent;
        private readonly IContext<LootboxEntity> lootboxContext;
        private readonly LootboxEcsController lootboxEcsController;
        private readonly IGroup<LootboxEntity> needToOpenLootboxGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(LootboxOpeningSystem));
        
        public LootboxOpeningSystem(IContext<LootboxEntity> lootboxContext, Transform contentParent, 
            LootboxEcsController lootboxEcsController) 
            : base(lootboxContext)
        {
            this.lootboxContext = lootboxContext;
            this.contentParent = contentParent;
            this.lootboxEcsController = lootboxEcsController;

            needToOpenLootboxGroup = lootboxContext.GetGroup(LootboxMatcher.NeedToOpenLootbox);
        }

        protected override ICollector<LootboxEntity> GetTrigger(IContext<LootboxEntity> context)
        {
            return context.CreateCollector(LootboxMatcher.CanvasClick);
        }

        protected override bool Filter(LootboxEntity entity)
        {
            return entity.isCanvasClick;
        }

        protected override void Execute(List<LootboxEntity> canvasClickEntities)
        {
            LootboxEntity needToOpenLootboxComponent = needToOpenLootboxGroup.GetEntities().SingleOrDefault();
            if (needToOpenLootboxComponent != null)
            {
                //Начать открывать лутбокс
                needToOpenLootboxComponent.needToOpenLootbox.lootboxOpeningController
                    .StartLootboxOpening(TryClick, contentParent);
                //Уничтожение нужно для того, чтобы не начали показываться ресурсы
                foreach (var entity in canvasClickEntities)
                {
                    entity.Destroy();
                }
                //Уничтожить компонент, чтобы при следующих кликах начали показываться призы
                needToOpenLootboxComponent.Destroy();
            }
        }

        /// <summary>
        /// Вызывается после окончания анимации открытия лутбокса.
        /// </summary>
        private void TryClick()
        {
            if (!lootboxEcsController.HasTheFirstResourceAlreadyBeenShown())
            {
                lootboxContext.CreateEntity().isCanvasClick = true;
            }
        }
        
        public List<Type> After()
        {
            return new List<Type>()
            {
                typeof(CanvasClickHandlerSystem)
            };
        }
    }
}