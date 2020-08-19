using System;
using Code.Common;
using Code.Scenes.LobbyScene.Scripts.Purchasing;
using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow;
using JetBrains.Annotations;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.Shop
{
    /// <summary>
    /// Реагирует на нажатие на товар в магазине.
    /// </summary>
    public class ProductClickHandlerScript : MonoBehaviour
    {
        private TextTooltip textTooltip;
        private PurchasingService purchasingService;
        private LobbyEcsController lobbyEcsController;
        private readonly ILog log = LogManager.CreateLogger(typeof(ProductClickHandlerScript));

        private void Awake()
        {
            purchasingService = FindObjectOfType<PurchasingService>()
                                ?? throw new NullReferenceException(nameof(PurchasingService));
            lobbyEcsController = FindObjectOfType<LobbyEcsController>()
                                 ?? throw new NullReferenceException(nameof(lobbyEcsController));
            textTooltip = FindObjectOfType<TextTooltip>()
                           ?? throw new NullReferenceException(nameof(TextTooltip));
        }

        public void Product_OnClick([NotNull] PurchaseModel purchaseModel)
        {
            UiSoundsManager.Instance().PlayClick();
            log.Info($"{nameof(Product_OnClick)} {nameof(purchaseModel.productModel.Id)} {purchaseModel.productModel.Id}");
            log.Info("Тип валюты "+purchaseModel.productModel.CostModel.CostTypeEnum);

            if (purchaseModel.productModel.IsDisabled)
            {
                textTooltip.Show("This product has already been purchased.");
                log.Info("Попытка купить недоступный товар.");
                return;
            }
            
            //Если покупка за реальную валюту, то вызвать api платёжной системы
            if (purchaseModel.productModel.CostModel.CostTypeEnum == CostTypeEnum.RealCurrency)
            {
                log.Info("Покупка за реальную валюту");
                purchasingService.BuyProduct(purchaseModel);
            }
            else
            {
                log.Info("Показ окна подтверждения покупки");
                //Если покупка за внутриигровую валюту, то показать меню подтверждения покупки

                lobbyEcsController.ShowPurchaseConfirmationWindow(purchaseModel);    
            }
        }

        public void DailyPresent_OnClick(PurchaseModel purchaseModel)
        {
            //TODO отправить запрос
            //TODO показать анимацию начисления
            //todo выключить подарок
        }
    }
}