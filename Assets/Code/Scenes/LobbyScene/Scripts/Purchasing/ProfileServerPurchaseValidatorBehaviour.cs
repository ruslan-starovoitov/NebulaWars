using Code.Common;
using System.Collections;
using System.Threading.Tasks;
using Code.Scenes.LobbyScene.Scripts.Purchasing;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts
{
    public class ProfileServerPurchaseValidatorBehaviour:MonoBehaviour
    {
        private PurchasingService purchasingService;
        private readonly ILog log = LogManager.CreateLogger(typeof(ProfileServerPurchaseValidatorBehaviour));

        private void Awake()
        {
            purchasingService = FindObjectOfType<PurchasingService>();
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