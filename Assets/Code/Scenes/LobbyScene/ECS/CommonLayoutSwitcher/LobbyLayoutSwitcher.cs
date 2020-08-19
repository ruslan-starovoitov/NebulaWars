using System;
using System.Collections.Generic;
using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher
{
    /// <summary>
    /// Хранит текущее состояние. Оно представлено перечислением ShittyUiLayerState.
    /// Реагирут на нажатие кнопки Back.
    /// </summary>
    public class LobbyLayoutSwitcher:ReactiveSystem<LobbyUiEntity>
    {
        private ShittyUiLayerState currentLayer;
        private readonly IContext<LobbyUiEntity> context;
        private readonly ILog log = LogManager.CreateLogger(typeof(LobbyLayoutSwitcher));
        
        public LobbyLayoutSwitcher(IContext<LobbyUiEntity> context)
            : base(context)
        {
            this.context = context;
            currentLayer = ShittyUiLayerState.DefaultLayer;
        }

        public ShittyUiLayerState GetCurrentLayer()
        {
            return currentLayer;
        }
        
        public void SetCurrentLayer(ShittyUiLayerState shittyUiLayerState)
        {
            // log.Debug("Установка нового слоя "+shittyUiLayerState);
            currentLayer = shittyUiLayerState;
        }
        
        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> contextArg)
        {
            return contextArg.CreateCollector(LobbyUiMatcher.BackButtonPressed);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.messageBackButtonPressed;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            switch (currentLayer)
            {
                case ShittyUiLayerState.DefaultLayer:
                    log.Debug("Недопустимое нажатие на кнопку назад. Текущий слой "+currentLayer);
                    break;
                case ShittyUiLayerState.ShopLayer:
                    context.CreateEntity().messageDisableShopUiLayer = true;
                    break;
                case ShittyUiLayerState.ShopPurchaseConfirmationLayer:
                    context.CreateEntity().isDisablePurchaseConfirmationWindow = true;
                    SetCurrentLayer(ShittyUiLayerState.ShopLayer);
                    break;
                case ShittyUiLayerState.WarshipsList:
                    context.CreateEntity().messageDisableWarshipListUiLayer = true;
                    break;
                case ShittyUiLayerState.WarshipOverview:
                    context.CreateEntity().messageDisableWarshipOverviewUiLayer = true;
                    break;
                case ShittyUiLayerState.WarshipOverviewModalWindow:
                    context.CreateEntity().messageDisableWarshipOverviewModalWindow = true;
                    break;
                case ShittyUiLayerState.WarshipImprovementModalWindow:
                    context.CreateEntity().messageDisableWarshipImprovementModalWindow = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentLayer));
            }
        }
    }

    public enum ShittyUiLayerState
    {
        DefaultLayer,
        ShopLayer,
        ShopPurchaseConfirmationLayer,
        WarshipsList,
        WarshipOverview,
        WarshipOverviewModalWindow,
        WarshipImprovementModalWindow
    }
}