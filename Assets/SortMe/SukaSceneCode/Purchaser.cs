// using Google.Play.Billing;
// using UnityEngine;
// using UnityEngine.Purchasing;
// using UnityEngine.Purchasing.Extension;
//
// namespace Code.Scenes.SukaScene
// {
//     public class Purchaser : MonoBehaviour, IStoreListener
//     {
//         // The Unity Purchasing system.
//         private static IStoreController storeController;          
//         // The store-specific Purchasing subsystems.
//         private static IExtensionProvider storeExtensionProvider; 
//         
//         private void Start()
//         {
//             if (storeController == null)
//             {
//                 InitializePurchasing();
//             }
//         }
//
//         private void InitializePurchasing() 
//         {
//             if (IsInitialized())
//             {
//                 return;
//             }
//
//             AbstractPurchasingModule test = GooglePlayStoreModule.Instance();
//             ConfigurationBuilder builder = ConfigurationBuilder.Instance(test);
//         
//             builder.AddProduct(PurchasingGlobals.Currency, ProductType.Consumable);
//             UnityPurchasing.Initialize(this, builder);
//         }
//
//         private bool IsInitialized()
//         {
//             return storeController != null && storeExtensionProvider != null;
//         }
//
//         public void ConfirmAll()
//         {
//             Product[] products = storeController.products.all;
//             foreach (var product in products)
//             {
//                 storeController.ConfirmPendingPurchase(product);
//             }
//         }
//     
//         public void BuyCurrency()
//         {
//             BuyProductId(PurchasingGlobals.Currency);
//         }
//     
//         private void BuyProductId(string productId)
//         {
//             if (IsInitialized())
//             {
//                 Product product = storeController.products.WithID(productId);
//                 if (product != null && product.availableToPurchase)
//                 {
//                     Debug.Log($"Purchasing product asychronously: '{product.definition.id}'");
//                     storeController.InitiatePurchase(product);
//                 }
//                 else
//                 {
//                     Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
//                 }
//             }
//             else
//             {
//                 Debug.Log("BuyProductID FAIL. Not initialized.");
//             }
//         }
//
//
//         //todo разобраться что делать с откатом.
//         // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
//         // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
//         // public void RestorePurchases()
//         // {
//         //     // If Purchasing has not yet been set up ...
//         //     if (!IsInitialized())
//         //     {
//         //         // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
//         //         Debug.Log("RestorePurchases FAIL. Not initialized.");
//         //         return;
//         //     }
//         //
//         //     // If we are running on an Apple device ... 
//         //     if (Application.platform == RuntimePlatform.IPhonePlayer || 
//         //         Application.platform == RuntimePlatform.OSXPlayer)
//         //     {
//         //         // ... begin restoring purchases
//         //         Debug.Log("RestorePurchases started ...");
//         //
//         //         // Fetch the Apple store-specific subsystem.
//         //         var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();
//         //         // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
//         //         // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
//         //         apple.RestoreTransactions((result) => {
//         //             // The first phase of restoration. If no more responses are received on ProcessPurchase then 
//         //             // no purchases are available to be restored.
//         //             Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
//         //         });
//         //     }
//         //     // Otherwise ...
//         //     else
//         //     {
//         //         // We are not running on an Apple device. No work is necessary to restore purchases.
//         //         Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
//         //     }
//         // }
//
//
//         //  
//         // --- IStoreListener
//         //
//
//         public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//         {
//             // Purchasing has succeeded initializing. Collect our Purchasing references.
//             Debug.Log("OnInitialized: PASS");
//         
//             storeController = controller;
//             storeExtensionProvider = extensions;
//         }
//     
//         public void OnInitializeFailed(InitializationFailureReason error)
//         {
//             Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
//         }
//     
//         public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
//         {
//             Debug.Log($"{nameof(ProcessPurchase)} Product id "+args.purchasedProduct.definition.id);
//        
//             return PurchaseProcessingResult.Complete;
//         }
//
//
//         public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//         {
//             Debug.Log(
//                 $"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
//         }
//     }
// }