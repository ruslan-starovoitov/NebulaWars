using System;
using Code.Common;
using Code.Scenes.LootboxScene.Scripts;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using System.Collections.Generic;
using Code.Common.Experimental.SystemsOrderChecker;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LootboxScene.ECS.Systems
{
    /// <summary>
    /// Реагирует на нажатие на канвас.
    /// </summary>
    public class CanvasClickHandlerSystem:ReactiveSystem<LootboxEntity>, IRequireSystemsInvocationOrderChecker
    {
        private int currentPrizeIndex;
        private List<ResourceModel> resourceModels;
        private readonly LootboxContext lootboxContext;
        private readonly UiSoundsManager uiSoundsManager;
        private readonly LootboxUiStorage lootboxUiStorage;
        private readonly LootboxSceneSwitcher lootboxLobbyLoaderController;
        private readonly ILog log = LogManager.CreateLogger(typeof(CanvasClickHandlerSystem));
        
        public CanvasClickHandlerSystem(Contexts contexts, LootboxSceneSwitcher lootboxLobbyLoaderController,
            UiSoundsManager uiSoundsManager, LootboxUiStorage lootboxUiStorage) 
            : base(contexts.lootbox)
        {
            currentPrizeIndex = -1;
            lootboxContext = contexts.lootbox;
            this.uiSoundsManager = uiSoundsManager;
            this.lootboxUiStorage = lootboxUiStorage;
            this.lootboxLobbyLoaderController = lootboxLobbyLoaderController;
        }

        public void SetResourceModels(List<ResourceModel> resourceModelsArg)
        {
            resourceModels = resourceModelsArg;
        } 
        
        protected override ICollector<LootboxEntity> GetTrigger(IContext<LootboxEntity> context)
        {
            return context.CreateCollector(LootboxMatcher.CanvasClick.Added());
        }

        protected override bool Filter(LootboxEntity entity)
        {
            return entity.isCanvasClick;
        }

        protected override void Execute(List<LootboxEntity> entities)
        {
            // log.Debug("Нажатие на канвас");
            if (resourceModels == null)
            {
                log.Info("Нажатие на канвас. Но ресурсы ещё не установлены.");
                return;
            }
            
            int numberOfClicks = entities.Count;
            for (int i = 0; i < numberOfClicks; i++)
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            currentPrizeIndex++;
            
            //Если все призы уже показаны
            if (currentPrizeIndex == resourceModels.Count)
            {
                //Вернуться в лобби
                lootboxUiStorage.resourcesRoot.transform.DestroyAllChildren();
                lootboxLobbyLoaderController.LoadLobbyScene();
                return;
            }
            
            //Если есть ресурсы, которые нужно показать
            uiSoundsManager.PlayAdding();
            ResourceModel resourceModel = resourceModels[currentPrizeIndex];
            LootboxEntity entity1 = lootboxContext.CreateEntity();
            entity1.AddShowPrize(resourceModel);

            
            LootboxEntity entity2 = lootboxContext.CreateEntity();
            int itemsLeftCount = resourceModels.Count - currentPrizeIndex-1;
            entity2.AddItemsLeft(itemsLeftCount);
        }

        public bool HasTheFirstResourceAlreadyBeenShown()
        {
            return currentPrizeIndex != -1;
        }
        
        public List<Type> After()
        {
            return new List<Type>()
            {
                typeof(ShowPrizeSystem),
                typeof(ItemsLeftChangedSystem)
            };
        }
    }
}
