using System.Collections;
using System.Collections.Generic;
using Code.Common;
using Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher;
using Code.Scenes.LobbyScene.Scripts.Shop;
using Code.Scenes.LobbyScene.Scripts.Shop.Spawners;
using Code.Scenes.LobbyScene.Scripts.UiStorages;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.ECS.Shop.Layer
{
    /// <summary>
    /// Реагирует на команду показать магазин
    /// </summary>
    public class ShopUiLayerEnablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly ShopUiSpawner shopUiSpawner;
        private readonly ShopUiStorage shopUiStorage;
        private readonly UiLayersStorage uiLayersStorage;
        private readonly LobbyLayoutSwitcher lobbyLayoutSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShopUiLayerEnablingSystem));
        
        public ShopUiLayerEnablingSystem(IContext<LobbyUiEntity> context, UiLayersStorage uiLayersStorage,
            ShopUiStorage shopUiStorage, LobbyLayoutSwitcher lobbyLayoutSwitcher, ShopUiSpawner shopUiSpawner)
            : base(context)
        {
            this.uiLayersStorage = uiLayersStorage;
            this.shopUiStorage = shopUiStorage;
            this.lobbyLayoutSwitcher = lobbyLayoutSwitcher;
            this.shopUiSpawner = shopUiSpawner;
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.EnableShopUiLayer);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.messageEnableShopUiLayer;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            EnableShopLayer();
            UnityThread.ExecuteCoroutine(SetStoreContentWidth());
            lobbyLayoutSwitcher.SetCurrentLayer(ShittyUiLayerState.ShopLayer);
        }
        
        public void EnableShopLayer()
        {
            log.Info(nameof(EnableShopLayer));
            
            uiLayersStorage.backButton.SetActive(true);
            uiLayersStorage.sectionText.SetActive(true);
            uiLayersStorage.sectionText.GetComponent<Text>().text = "SHOP";
            uiLayersStorage.layer0RootGameObject.SetActive(false);
            uiLayersStorage.lootboxRootGameObject.SetActive(false);
            uiLayersStorage.warshipsRootGameObject.SetActive(false);
            uiLayersStorage.warshipsUiLayerRootGameObject.SetActive(false);
            uiLayersStorage.shopLayerRootGameObject.SetActive(true);
        }
        
        /// <summary>
        /// Это нужно выполнять с задержкой из-за того, что для выравнивания разделов используется Layout group.
        /// Автоматической выравнивание происходит после включения слоя.
        /// До этого момента ui магазина мог быть выключен. А значит, что сейчас там может быть каша из разделов.
        /// Нужно подождать один кард чтобы произошло выравнивание и только потом брать ширину. 
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetStoreContentWidth()
        {
            yield return null;
            RectTransform sectionsParentRect = shopUiStorage.shopSectionsParent.GetComponent<RectTransform>();
            shopUiStorage.shopScrollViewContent
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sectionsParentRect.rect.width);
            
            //обновить список указателей
            shopUiSpawner.SpawnFooterPointers();
        }
    }
}