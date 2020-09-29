// using System;
// using System.Collections.Generic;
// using Code.Common;
// using Code.Common.Storages;
// using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow;
// using NetworkLibrary.NetworkLibrary.Http;
// using Plugins.submodules.SharedCode.Logger;
// using Plugins.submodules.SharedCode.NetworkLibrary.Udp;
// using UnityEngine;
// using UnityEngine.Purchasing;
// using UnityEngine.Purchasing.Extension;
// using UnityEngine.Purchasing.Security;
// using ZeroFormatter;
// using IGooglePlayStoreExtensions = Google.Play.Billing.IGooglePlayStoreExtensions;
//
// namespace Code.Scenes.LobbyScene.Scripts.Purchasing
// {
//     /// <summary>
//     /// Отвечает за взаимодействие с платёжной системой
//     /// </summary>
//     [RequireComponent(typeof(ProfileServerPurchaseValidatorBehaviour))]
//     public class PurchasingService : MonoBehaviour, IStoreListener, IPurchasingService
//     {
//         private IStoreController storeController;
//         private IExtensionProvider extensionsProvider;
//         private ProfileServerPurchaseValidatorBehaviour serverValidator;
//         private readonly ILog log = LogManager.CreateLogger(typeof(PurchasingService));
//
//         private void Awake()
//         {
//             serverValidator = GetComponent<ProfileServerPurchaseValidatorBehaviour>();
//         }
//
//         public void StartInitialization(List<RealCurrencyCostModel> serviceProducts)
//         {
// #warning нужно использовать полное название пространства имён
//             AbstractPurchasingModule purchasingModule = Google.Play.Billing.GooglePlayStoreModule.Instance();
// #warning нужно использовать полное название пространства имён
//             
//             ConfigurationBuilder builder = ConfigurationBuilder.Instance(purchasingModule);
//             foreach (RealCurrencyCostModel realCurrencyCostModel in serviceProducts)
//             {
//                 string productId = realCurrencyCostModel.GoogleProductId;
//                 ProductType productType = realCurrencyCostModel.IsConsumable
//                     ? ProductType.Consumable
//                     : ProductType.NonConsumable;
//                 log.Debug($"{nameof(productId)} {productId} {nameof(productType)} {productType}");
//                 builder.AddProduct(productId, productType);
//             }
//          
//             UnityPurchasing.Initialize(this, builder);
//         }
//         
//         public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//         {
//             log.Info(nameof(OnInitialized));
//             storeController = controller;
//             extensionsProvider = extensions;
//             if (!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
//             {
//                 log.Error("Не удалось достать playerServiceId");
//                 return;
//             }
//             IGooglePlayStoreExtensions playStoreExtensions =
//                 extensionsProvider.GetExtension<IGooglePlayStoreExtensions>();
//
//             string obfuscatedAccountId = playerServiceId.Caesar();
//             playStoreExtensions.SetObfuscatedAccountId(obfuscatedAccountId);
//         }
//         
//         public void OnInitializeFailed(InitializationFailureReason error)
//         {
//             log.Debug($"{nameof(OnInitializeFailed)} "+error);
//         }
//
//         public bool IsStoreInitialized()
//         {
//             return storeController != null && extensionsProvider != null;
//         }
//
//         public void BuyProduct(PurchaseModel purchaseModel)
//         {
//             var realCurrencyCostModel =
//                 ZeroFormatterSerializer.Deserialize<RealCurrencyCostModel>(purchaseModel.productModel.CostModel
//                     .SerializedCostModel);
//             string sku = realCurrencyCostModel.GoogleProductId;
//             log.Debug($"{nameof(sku)} "+sku);
//             bool isStoreInitialized = IsStoreInitialized(); 
//             log.Debug($"{nameof(isStoreInitialized)} {isStoreInitialized}");
//             if (!isStoreInitialized)
//             {
//                 UiSoundsManager.Instance().PlayError();
//                 log.Error($"Вызов покупки был сделан до того как магазин " +
//                           $"был инициализирован. {nameof(sku)} {sku}");
//             }
//
//             Product product = storeController.products.WithID(sku);
//             if (product == null || !product.availableToPurchase)
//             {
//                 log.Error($"product is null or not available for purchase");
//             }
//
//             if (!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
//             {
//                 UiSoundsManager.Instance().PlayError();
//                 log.Error($"Не удалось выполнить покупку потому, что {nameof(playerServiceId)} не доступен");
//             }
//
//             if (playerServiceId == null)
//             {
//                 log.Error($"{nameof(playerServiceId)} is null");
//                 throw new Exception($"{nameof(playerServiceId)} is null");
//             }
//             
//             storeController.InitiatePurchase(sku);
//         }
//
//         public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEventArgs)
//         {   
//              log.Debug(nameof(ProcessPurchase));
//              string receipt = purchaseEventArgs.purchasedProduct.receipt;
//           
//              bool purchaseIsValid = true;
//              string sku = purchaseEventArgs.purchasedProduct.definition.id;
//              string token = null;
//              
//              
// #if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
//             CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
//                 AppleTangle.Data(), Application.identifier);
//
//             purchaseEventArgs.purchasedProduct.LogProduct(log);
//
//             try
//             {
//                 IPurchaseReceipt[] result = validator.Validate(purchaseEventArgs.purchasedProduct.receipt);
//
//                 log.Debug("Receipt is valid. Contents:");
//                 foreach (IPurchaseReceipt productReceipt in result)
//                 {
//                     log.Debug(productReceipt.productID);
//                     log.Debug(productReceipt.purchaseDate.ToString());
//                     log.Debug(productReceipt.transactionID);
//                     if (productReceipt is GooglePlayReceipt google)
//                     {
//                         log.Debug(google.transactionID);
//                         log.Debug(google.purchaseState.ToString());
//                         log.Debug(google.purchaseToken);
//                         token = google.purchaseToken;
//                     }
//                 }
//             }
//             catch (IAPSecurityException)
//             {
//                 log.Error("Invalid receipt, not unlocking content");
//                 purchaseIsValid = false;
//             }
//             catch (Exception e)
//             {
//                 log.Error(e.Message+" "+e.StackTrace);
//             }
// #endif
//             if (!purchaseIsValid)
//             {
//                 log.Error($"Покупка не прошла локальную проверку");
//                 UiSoundsManager.Instance().PlayError();
//             }
//             
//             serverValidator.StartValidation(sku, token);
//             return PurchaseProcessingResult.Pending;    
//         }
//
//         public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
//         {
//             log.Debug($"{nameof(OnPurchaseFailed)} "+p+" "+i.definition.id);
//         }
//
//         public void ConfirmAll()
//         {
//             log.Debug(nameof(ConfirmAll));
//             var products = storeController.products.all;
//             foreach (var product in products)
//             {
//                 storeController.ConfirmPendingPurchase(product);
//                 log.Debug("success confirm "+product.definition.id);
//             }
//         }
//         
//         public bool TryConfirmPendingPurchase(string sku)
//         {
//             bool success = false;
//             log.Debug(nameof(TryConfirmPendingPurchase));
//             try
//             {
//                 Product product = storeController.products.WithID(sku);
//                 log.Debug($"Продукт найден по id {nameof(sku)} {sku}");
//                 log.Debug("Подтверждение транзакции " + product.definition.id);
//                 storeController.ConfirmPendingPurchase(product);
//                 log.Debug("Транзакция успешно подтверджена");
//                 success = true;
//             }
//             catch (Exception e)
//             {
//                 UiSoundsManager.Instance().PlayError();
//                 log.Error($"Не удалось подтвердить продукт {nameof(sku)} {sku}." +
//                           $" {e.Message} {e.StackTrace}");
//             }
//         
//             return success;
//         }
//         
//         public bool TryGetProductCostById(string sku, ref string cost)
//         {
//             try
//             {
//                 Product product = storeController.products.WithID(sku);
//                 cost = product.metadata.localizedPriceString;
//                 return true;
//             }
//             catch (Exception e)
//             {
//                 UiSoundsManager.Instance().PlayError();
//                 log.Error($"Не удалось получить стоимость продукта через api. " +
//                           $"{nameof(sku)} {sku} {e.Message} {e.StackTrace}");
//             }
//
//             return false;
//         }
//     }
// }
