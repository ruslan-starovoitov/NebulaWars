using Code.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Scenes.LobbyScene.Scripts.Shop;
using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts
{
    public interface IPurchasingService
    {
        void StartInitialization(List<RealCurrencyCostModel> serviceProducts);
        bool IsStoreInitialized();
        void BuyProduct(PurchaseModel purchaseModel);
        bool TryConfirmPendingPurchase(string sku);
        bool TryGetProductCostById(string sku, ref string cost);
        void BuyCurrency();
        void ConfirmAll();
    }
    public class ProfileServerPurchaseValidatorBehaviour:MonoBehaviour
    {
        private IPurchasingService purchasingService;
        private readonly ILog log = LogManager.CreateLogger(typeof(ProfileServerPurchaseValidatorBehaviour));

        private void Awake()
        {
            purchasingService = FindObjectOfType<PurchasingServiceStub>();
        }

        public void StartValidation(string sku, string token)
        {
            StartCoroutine(ValidateCoroutine(sku, token));
        }

        private IEnumerator ValidateCoroutine(string sku, string token)
        {
            Task<bool> task = new PurchaseValidatorTaskFactory().Create(sku, token);
            yield return new WaitUntil(()=>task.IsCompleted);
            if (task.IsCanceled || task.IsFaulted)
            {
                log.Error($"Не удалось подтвердить покупку " +
                          $"{nameof(task.IsCanceled)} {task.IsCanceled}" +
                          $"{nameof(task.IsFaulted)} {task.IsFaulted}");
                UiSoundsManager.Instance().PlayError();
                yield break;
            }

            bool success = purchasingService.TryConfirmPendingPurchase(sku);
            if (!success)
            {
                log.Error($"Сервис платёжной системы не завершил покупку.");
                UiSoundsManager.Instance().PlayError();
            }
        }
    }
}