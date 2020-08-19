using System.Collections.Generic;
using Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher;
using Code.Scenes.LobbyScene.Scripts.UiStorages;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.Shop.Layer
{
    /// <summary>
    /// Реагирует на команду скрыть магазин
    /// </summary>
    public class ShopUiLayerDisablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly UiLayersStorage uiLayersStorage;
        private readonly LobbyLayoutSwitcher lobbyLayoutSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShopUiLayerDisablingSystem));
        
        public ShopUiLayerDisablingSystem(IContext<LobbyUiEntity> context, UiLayersStorage uiLayersStorage,
            LobbyLayoutSwitcher lobbyLayoutSwitcher)
            : base(context)
        {
            this.uiLayersStorage = uiLayersStorage;
            this.lobbyLayoutSwitcher = lobbyLayoutSwitcher;
        }
        
        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.DisableShopUiLayer);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.messageDisableShopUiLayer;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            DisableShopLayer();
            lobbyLayoutSwitcher.SetCurrentLayer(ShittyUiLayerState.DefaultLayer);
        }
        
        public void DisableShopLayer()
        {
            uiLayersStorage.backButton.SetActive(false);  
            uiLayersStorage.sectionText.SetActive(false);
            
            uiLayersStorage.layer0RootGameObject.SetActive(true);
            uiLayersStorage.lootboxRootGameObject.SetActive(true);
            uiLayersStorage.warshipsRootGameObject.SetActive(true);
            
            uiLayersStorage.shopLayerRootGameObject.SetActive(false);
        }
    }
}