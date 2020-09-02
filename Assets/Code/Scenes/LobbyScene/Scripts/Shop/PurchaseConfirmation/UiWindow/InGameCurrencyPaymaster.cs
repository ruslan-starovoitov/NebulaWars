using System;
using System.Net.Http;
using Code.Common;
using Code.Common.Storages;
using Code.Scenes.LobbyScene.Scripts.ResourcesAccrual;
using Code.Scenes.LobbyScene.Scripts.Shop.Spawners;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using ZeroFormatter;

namespace Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow
{
    /// <summary>
    /// При покупке за игровую валюту отправляет запрос на сервер для проведения транзакции.
    /// </summary>
    [RequireComponent(typeof(InsufficientResourceErrorHandler))]
    public class InGameCurrencyPaymaster:MonoBehaviour
    {
        private SectionSpawner sectionSpawner;
        private LobbyEcsController lobbyEcsController;
        private ResourcesAccrualSceneManager resourcesAccrualSceneManager;
        private InsufficientResourceErrorHandler insufficientResourceErrorHandler;
        private readonly ILog log = LogManager.CreateLogger(typeof(InGameCurrencyPaymaster));

        private void Awake()
        {
            sectionSpawner = FindObjectOfType<SectionSpawner>();
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
            resourcesAccrualSceneManager = FindObjectOfType<ResourcesAccrualSceneManager>();
            insufficientResourceErrorHandler = FindObjectOfType<InsufficientResourceErrorHandler>();
        }

        public void StartBuying(PurchaseModel purchaseModel)
        {
            log.Info("начало покупки");
            //если не хватает ресурсов 
            if (InsufficientResources(purchaseModel.productModel, out var insufficientResource))
            {
                //показать уведомление и переместить магазин на секцию с нужными ресурсами
                if (insufficientResource == null)
                {
                    throw new Exception("Ошибка при определении недостающего ресурса");
                }
                
                insufficientResourceErrorHandler.ShowError(insufficientResource.Value);
                return;
            }
            
            log.Info("ресурсов хватает ");
            
            //отправить блокирующий запрос на сервер
            if(!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
            {
                log.Error("Не удалось получить id игрока");
                return;
            }

            byte[] binaryProductModel = ZeroFormatterSerializer.Serialize(purchaseModel.productModel);
            string base64ProductModel = Convert.ToBase64String(binaryProductModel);
                    
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            using (MultipartFormDataContent formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(playerServiceId), "playerId");   
                formData.Add(new StringContent(purchaseModel.productModel.Id.ToString()), "productId");   
                formData.Add(new StringContent(base64ProductModel), "base64ProductModel");   
                formData.Add(new StringContent(purchaseModel.shopModelId.ToString()), "shopModelId");   
                response = httpClient.PostAsync(NetworkGlobals.BuyProduct, formData).Result;
            }

            if (response == null)
            {
                log.Error("Пустой ответ от сервера");
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                log.Error("Статус ответа от сервера "+ response.StatusCode);
                return;
            }
            
            log.Info("Операция покупки прошла успешно");
            //todo показать аниме
            resourcesAccrualSceneManager.ShowOneResource(purchaseModel);
            sectionSpawner.DisableProduct(purchaseModel.productModel.Id);
        }

        private bool InsufficientResources(ProductModel productModel, out SectionTypeEnum? sectionTypeEnum)
        {
            switch (productModel.CostModel.CostTypeEnum)
            {
                case CostTypeEnum.SoftCurrency:
                {
                    var costModel = ZeroFormatterSerializer
                        .Deserialize<InGameCurrencyCostModel>(productModel.CostModel.SerializedCostModel);
                    if (lobbyEcsController.GetSoftCurrency() < costModel.Cost)
                    {
                        log.Error("Не хватает обычной валюты");
                        sectionTypeEnum = SectionTypeEnum.SoftCurrency;
                        return true;
                    }

                    sectionTypeEnum = null;
                    return false;
                }
                case CostTypeEnum.HardCurrency:
                {
                    var costModel = ZeroFormatterSerializer
                        .Deserialize<InGameCurrencyCostModel>(productModel.CostModel.SerializedCostModel);
                    log.Debug($"текущее кол-во хард валюты {lobbyEcsController.GetHardCurrency()} а нужно {costModel.Cost}");
                    if (lobbyEcsController.GetHardCurrency() < costModel.Cost)
                    {
                        log.Error("Не хватает премиум валюты");
                        sectionTypeEnum = SectionTypeEnum.HardCurrency;
                        return true;
                    }

                    sectionTypeEnum = null;
                    return false;
                }
                case CostTypeEnum.RealCurrency:
                    log.Error("Покупка за реальную валюту не должна тут обрабатываться");
                    sectionTypeEnum = null;
                    return true;
                case CostTypeEnum.Free:
                    sectionTypeEnum = null;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}