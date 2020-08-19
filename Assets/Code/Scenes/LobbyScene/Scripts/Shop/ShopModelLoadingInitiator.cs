using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Scenes.LobbyScene.Scripts.Purchasing;
using Code.Scenes.LobbyScene.Scripts.Shop.Spawners;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using ZeroFormatter;


namespace Code.Scenes.LobbyScene.Scripts.Shop
{
    /// <summary>
    /// Точка входа в скрипты магазина.
    /// Скачивает данные для разметки магазина при входе в игру.
    /// </summary>
    public class ShopModelLoadingInitiator : MonoBehaviour
    {
        private ShopUiSpawner shopUiSpawner;
        private CancellationTokenSource cts;
        private PurchasingService purchasingService;
        private LobbyEcsController lobbyEcsController;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShopModelLoadingInitiator));

        private void Awake()
        {
            shopUiSpawner = FindObjectOfType<ShopUiSpawner>()
                    ?? throw new NullReferenceException(nameof(ShopUiSpawner));
            purchasingService = FindObjectOfType<PurchasingService>()
                    ?? throw new NullReferenceException(nameof(PurchasingService));
            lobbyEcsController = FindObjectOfType<LobbyEcsController>()
                    ?? throw new NullReferenceException(nameof(PurchasingService));
        }
        
        private void Start()
        {
            StartShopLoading();
        }

        public void StartShopLoading()
        {
            StartCoroutine(LoadShop());   
        }

        private IEnumerator LoadShop()
        {
            cts = new CancellationTokenSource();
            Task<ShopModel> loadShopModelTask = new ShowModelDownloader().GetShopModelAsync(cts.Token);
            yield return new WaitUntil(()=>loadShopModelTask.IsCompleted);
            if (loadShopModelTask.IsCanceled || loadShopModelTask.IsFaulted)
            {
                log.Error($"Не удалось скачать содержимое магазина. " +
                          $"{nameof(loadShopModelTask.IsCanceled)} {loadShopModelTask.IsCanceled}" +
                          $"{nameof(loadShopModelTask.IsFaulted)} {loadShopModelTask.IsFaulted}");
                yield break;
            }

            ShopModel shopModel = loadShopModelTask.Result;
            var wppInitTask = InitWppModel(shopModel);
            yield return new WaitUntil(()=>wppInitTask.IsCompleted);
            if (wppInitTask.IsCanceled || wppInitTask.IsFaulted)
            {
                log.Error($"Не удалось получить данные для продуктов с очками силы. " +
                          $"{nameof(wppInitTask.IsCanceled)} {wppInitTask.IsCanceled} " +
                          $"{nameof(wppInitTask.IsFaulted)} {wppInitTask.IsFaulted}");
                yield break;
            }
            
            shopModel = wppInitTask.Result;
#if !UNITY_EDITOR && UNITY_ANDROID
            Task<ShopModel> initProductCostTask = InitProductCostAsync(shopModel);
            yield return new WaitUntil(()=>initProductCostTask.IsCompleted);
            shopModel = initProductCostTask.Result;
#endif
            shopUiSpawner.Spawn(shopModel);
        }

        private async Task<ShopModel> InitWppModel(ShopModel shopModel)
        {
            // log.Debug("Ожидание спавна кораблей");
            await MyTaskExtensions.WaitUntil(lobbyEcsController.IsWarshipsCreationCompleted);
            // log.Debug(lobbyEcsController.GetCountOfSpawnedWarships());
            // log.Debug("Ожидание спавна закончено");
            var products = shopModel.UiSections.Select(section => section.UiItems)
                .SelectMany(item => item)
                .SelectMany(item=>item)
                .ToList()
                ;
        
            // log.Debug("Кол-во продуктов "+products.Count());
            foreach (ProductModel productModel in products)
            {
                if (productModel.ResourceTypeEnum == ResourceTypeEnum.WarshipPowerPoints)
                {
                    // log.Debug("Упаковка вспомогательной информации для продукта с id "+productModel.Id);
                    try
                    {
                        WarshipPowerPointsProductModel model = ZeroFormatterSerializer
                            .Deserialize<WarshipPowerPointsProductModel>(productModel.SerializedModel);
                        
                        // log.Debug($"{nameof(model.Increment)} {model.Increment}" +
                        //           $"{nameof(model.WarshipId)} {model.WarshipId}"+ 
                        //           $"{nameof(model.SupportClientModel)} {model.SupportClientModel}"+ 
                        //           $"{nameof(model.WarshipTypeEnum)} {model.WarshipTypeEnum}"+ 
                        //           "");
                        int powerLevel = lobbyEcsController.GetWarshipPowerLevel(model.WarshipTypeEnum);
                        var powerModel = WarshipPowerScale.GetModel(powerLevel);
        
                        var supportModel = new WppSupportClientModel()
                        {
                            StartValue = lobbyEcsController.GetWarshipPowerPoints(model.WarshipTypeEnum),
                            WarshipSkinName = lobbyEcsController.GetSkinName(model.WarshipTypeEnum),
                            MaxValueForLevel = powerModel.PowerPointsCost
                        };
        
                        model.SupportClientModel = supportModel;
        
                        productModel.SerializedModel = ZeroFormatterSerializer.Serialize(model);
                    }
                    catch (Exception e)
                    {
                        log.Error(e.Message+ " "+e.StackTrace);
                        return null;
                    }
                }
            }
        
            return shopModel;
        }
        
        private async Task<ShopModel> InitProductCostAsync(ShopModel shopModel)
        {
            log.Debug($"Ожидание инициализации {nameof(purchasingService)}");
            List<RealCurrencyCostModel> realCurrencyProducts = shopModel.GetRealCurrencyProducts();
            purchasingService.StartInitialization(realCurrencyProducts);
            await MyTaskExtensions.WaitUntil(purchasingService.IsStoreInitialized);
            log.Debug($"{nameof(purchasingService)} инициализирован");
            foreach (ProductModel productModel in shopModel.GetAllProducts())
            {
                if (productModel.CostModel.CostTypeEnum == CostTypeEnum.RealCurrency)
                {
                    var realCurrencyCostModel = ZeroFormatterSerializer
                        .Deserialize<RealCurrencyCostModel>(productModel.CostModel.SerializedCostModel);
                    string productId = realCurrencyCostModel.GoogleProductId;
                    string cost = null;
                    purchasingService.TryGetProductCostById(productId, ref cost);
                    if (cost == null)
                    {
                        throw new Exception("Не удалось достать цену товара из плагина магазина");
                    }

                    
                    realCurrencyCostModel.CostString = cost;
                    productModel.CostModel.SerializedCostModel =
                        ZeroFormatterSerializer.Serialize(realCurrencyCostModel);
                }    
            }

            return shopModel;
        }
        
        private void OnDestroy()
        {
            cts?.Cancel();
        }
    }
}