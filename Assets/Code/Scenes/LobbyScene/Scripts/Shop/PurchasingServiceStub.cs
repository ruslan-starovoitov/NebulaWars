using System.Collections.Generic;
using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow;
using NetworkLibrary.NetworkLibrary.Http;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.Shop
{
    public class PurchasingServiceStub:MonoBehaviour, IPurchasingService
    {
        public void StartInitialization(List<RealCurrencyCostModel> serviceProducts)
        {
        }

        public bool IsStoreInitialized()
        {
            return true;
        }

        public void BuyProduct(PurchaseModel purchaseModel)
        {}

        public bool TryConfirmPendingPurchase(string sku)
        {
            return true;
        }

        public bool TryGetProductCostById(string sku, ref string cost)
        {
            cost = "NaN";
            return true;
        }

        public void BuyCurrency()
        {
            
        }

        public void ConfirmAll()
        {
            
        }
    }
}