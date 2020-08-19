using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Code.Scenes.LobbyScene.Scripts.AccountModel;
using Code.Scenes.LobbyScene.Scripts.Listeners;
using Code.Scenes.LobbyScene.Scripts.Shop;
using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow;
using Code.Scenes.LootboxScene.Scripts;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scenes.LobbyScene.Scripts.ResourcesAccrual
{
    /// <summary>
    /// todo это говнокод
    /// </summary>
    public class ResourcesAccrualSceneManager:MonoBehaviour
    {
        private CancellationTokenSource cts;
        private LobbyEcsController lobbyEcsController;
        private Task<LobbyModel> lobbyModelDownloadingTask;
        private ShopModelLoadingInitiator shopModelLoadingInitiator;
        private readonly ILog log = LogManager.CreateLogger(typeof(ResourcesAccrualSceneManager));

        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
            shopModelLoadingInitiator = FindObjectOfType<ShopModelLoadingInitiator>();
        }

        public void ShowLootboxScene()
        {
            LootboxModelDownloader.Instance.StartDownloading();
            DisableLobbyUi();
            SceneManager.LoadScene("2dLootboxScene", LoadSceneMode.Additive);
            
            cts?.Cancel();
            cts = new CancellationTokenSource();
            lobbyModelDownloadingTask = new LobbyModelLoader().Load(cts.Token);

         
            
            SceneManager.sceneUnloaded += LootboxSceneClosed;
            ResourcesAccrualStorage.Instance.Clear();
            StartCoroutine(SetLootboxResources());
        }

       
        public void ShowOneResource(PurchaseModel purchaseModel)
        {
            DisableLobbyUi();
            SceneManager.LoadScene("2dLootboxScene", LoadSceneMode.Additive);
            SceneManager.sceneUnloaded += OneResourceSceneClosed;
            ResourcesAccrualStorage.Instance.Clear();
            var resourceModel = new ResourceModelMapper().Map(purchaseModel);
            ResourcesAccrualStorage.Instance.SetResourcesModels(new List<ResourceModel>()
            {
                resourceModel
            });
            ResourcesAccrualStorage.Instance.SetNoLootboxNeeded();
            lobbyEcsController.ClosePurchaseConfirmationWindow();
            lobbyEcsController.CloseShopLayer();
            
            cts?.Cancel();
            cts = new CancellationTokenSource();
            lobbyModelDownloadingTask = new LobbyModelLoader().Load(cts.Token);
        }

        private void OneResourceSceneClosed(Scene arg0)
        {
            SceneManager.sceneUnloaded -= OneResourceSceneClosed;
            EnableLobbyUi();
            TryUpdateAccountModel();
            shopModelLoadingInitiator.StartShopLoading();
        }
        
        private void LootboxSceneClosed(Scene arg0)
        {
            SceneManager.sceneUnloaded -= LootboxSceneClosed;
            EnableLobbyUi();
            TryUpdateAccountModel();
            shopModelLoadingInitiator.StartShopLoading();
        }

        private void TryUpdateAccountModel()
        {
            if (lobbyModelDownloadingTask.IsCompleted)
            {
                if (lobbyModelDownloadingTask.IsCanceled)
                {
                    return;
                }
                if (!lobbyModelDownloadingTask.IsFaulted)
                {
                    LobbyModel lobbyModel = lobbyModelDownloadingTask.Result;
                    if (lobbyModel != null)
                    {
                        log.Info("Установка новой модели аккаунта");
                        lobbyEcsController.SetLobbyModel(lobbyModel);
                    }
                    else
                    {
                        log.Error("Модель лобби пуста");
                    }
                }
                else
                {
                    log.Error("Новая модель лобби не успела скачаться пока показывалась красивая анимация");
                }
            }
            else
            {
                log.Info("Новая модель аккаунта не успела скачаться.");
                lobbyModelDownloadingTask.ContinueWith(task =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        LobbyModel lobbyModel = task.Result;
                        if (lobbyModel != null)
                        {
                            lobbyEcsController.SetLobbyModel(lobbyModel);
                        }
                        else
                        {
                            log.Error("Пришла пустая модель лобби.");
                        }
                    }
                    else
                    {
                        log.Error("Не удалось скачать модель лобби.");
                    }
                });
            }
        }
        
        private IEnumerator SetLootboxResources()
        {
            ResourcesAccrualStorage.Instance.SetLootboxNeeded();
            yield return new WaitUntil(() => LootboxModelDownloader.Instance.IsDownloadingCompleted());
            LootboxModel lootboxPrize = LootboxModelDownloader.Instance.GetLootboxModel();
            if (lootboxPrize != null)
            {
                var test = lootboxPrize.Prizes;
                ResourcesAccrualStorage.Instance.SetResourcesModels(test);
            }
        }

        private void DisableLobbyUi()
        {
            lobbyEcsController.DisableLobbySceneUi();
        }
        
        private void EnableLobbyUi()
        {
            lobbyEcsController.EnableLobbySceneUi();
        }
    }
}