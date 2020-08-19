using Code.Common;
using Code.Scenes.LootboxScene.ECS.Systems;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Experimental.SystemsOrderChecker;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LootboxScene.Scripts
{
   /// <summary>
    /// Создаёт/вызывает системы.
    /// Пропускает через себя нажатия на экран.
    /// </summary>
    public class LootboxEcsController : MonoBehaviour
    {
        private Systems systems;
        private Contexts contexts;
        private LootboxUiStorage lootboxUiStorage;
        private LootboxSceneSwitcher lobbyLoaderController;
        private ParticlesColorUpdater particlesColorUpdater;
        private CanvasClickHandlerSystem canvasClickHandlerSystem;
        private readonly ILog log = LogManager.CreateLogger(typeof(LootboxEcsController));

        private void Awake()
        {
            lobbyLoaderController = FindObjectOfType<LootboxSceneSwitcher>()
                                ?? throw new NullReferenceException(nameof(LootboxSceneSwitcher));
            lootboxUiStorage = FindObjectOfType<LootboxUiStorage>()
                                ?? throw new NullReferenceException(nameof(LootboxUiStorage));
            particlesColorUpdater = FindObjectOfType<ParticlesColorUpdater>()
                                ?? throw new NullReferenceException(nameof(ParticlesColorUpdater));
        }

        private void Start()
        {
            contexts = Contexts.sharedInstance;
            canvasClickHandlerSystem = new CanvasClickHandlerSystem(contexts, lobbyLoaderController, 
                UiSoundsManager.Instance(), lootboxUiStorage);
            
            SystemsContainer systemsContainer = new SystemsContainer()
                .Add(new LootboxSpawnSystem(contexts.lootbox, lootboxUiStorage.lootboxPrefab,
                    lootboxUiStorage.resourcesRoot.transform))
                .Add(new LootboxOpeningSystem(contexts.lootbox, lootboxUiStorage.resourcesRoot.transform, this))
                .Add(canvasClickHandlerSystem)
                .Add(new ShowPrizeSystem(contexts, particlesColorUpdater, lootboxUiStorage))
                .Add(new ItemsLeftChangedSystem(contexts, lootboxUiStorage))
                .Add(new ItemsLeftDisablingSystem(contexts, lootboxUiStorage))
                .Add(new ResourcesInitializeSystem(lootboxUiStorage))
                ;

            systems = systemsContainer.GetSystems();
            systems.Initialize();
        }

        private void Update()
        {
            systems.Execute();
            systems.Cleanup();
        }

        private void OnDestroy()
        {
            if (systems != null)
            {
                systems.DeactivateReactiveSystems();
                systems.TearDown();
                contexts.lootbox.DestroyAllEntities();
                systems.ClearReactiveSystems();    
            }
        }

        public void CanvasButton_OnClick()
        {
            log.Info("Click");
            LootboxEntity entity = contexts.lootbox.CreateEntity();
            entity.isCanvasClick = true;
        }
        
        public void SetResourceModels(List<ResourceModel> resourceModels)
        {
            canvasClickHandlerSystem.SetResourceModels(resourceModels);
            if (resourceModels.Count == 1)
            {
                contexts.lootbox.CreateEntity().isCanvasClick = true;
            }
        }

        public void ShowLootbox()
        {
            contexts.lootbox.CreateEntity().isShowLootbox = true;
        }

        public bool HasTheFirstResourceAlreadyBeenShown()
        {
            return canvasClickHandlerSystem.HasTheFirstResourceAlreadyBeenShown();
        }

        public void DisableItemsLeftMenu()
        {
            contexts.lootbox.CreateEntity().isDisableItemsLeftMenu = true;
        }
    }
}