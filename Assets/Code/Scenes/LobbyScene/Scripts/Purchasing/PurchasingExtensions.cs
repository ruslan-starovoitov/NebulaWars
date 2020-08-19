using Code.Common;
using System;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine.Purchasing;

namespace Code.Scenes.LobbyScene.Scripts
{
    public static class PurchasingExtensions
    {
        public static void LogProduct(this Product purchasedProduct, ILog log)
        {
            log.Debug(nameof(LogProduct)+" "+purchasedProduct.definition.id);
            try
            {
                var definitionEnabled = purchasedProduct.definition.enabled;
                var definitionId = purchasedProduct.definition.id;
                var definitionPayout = purchasedProduct.definition.payout;

                if (purchasedProduct.definition.payout != null)
                {
                    var payoutData = purchasedProduct.definition.payout.data;
                    var payoutQuantity = purchasedProduct.definition.payout.quantity;
                    var payoutSubtype = purchasedProduct.definition.payout.subtype;
                    var payoutTypeString = purchasedProduct.definition.payout.typeString;
                    
                    // log.Debug($"{nameof(payoutData)} {payoutData}");
                    // log.Debug($"{nameof(payoutQuantity)} {payoutQuantity}");
                    // log.Debug($"{nameof(payoutSubtype)} {payoutSubtype}");
                    // log.Debug($"{nameof(payoutTypeString)} {payoutTypeString}");
                }
                
                var purchasedProductDefinition = purchasedProduct.definition;
                var purchasedProductReceipt = purchasedProduct.receipt;
                var purchasedProductHasReceipt = purchasedProduct.hasReceipt;
                var purchasedProductTransactionId = purchasedProduct.transactionID;
                var metadataLocalizedPrice = purchasedProduct.metadata.localizedPrice;
                var metadataLocalizedTitle = purchasedProduct.metadata.localizedTitle;
                var metadataIsoCurrencyCode = purchasedProduct.metadata.isoCurrencyCode;
                var metadataLocalizedPriceString = purchasedProduct.metadata.localizedPriceString;
                
                // log.Debug($"{nameof(definitionEnabled)} {definitionEnabled}");
                // log.Debug($"{nameof(definitionId)} {definitionId}");
                // log.Debug($"{nameof(definitionPayout)} {definitionPayout}");
                // log.Debug($"{nameof(purchasedProductDefinition)} {purchasedProductDefinition}");
                // log.Debug($"{nameof(purchasedProductReceipt)} {purchasedProductReceipt}");
                // log.Debug($"{nameof(purchasedProductHasReceipt)} {purchasedProductHasReceipt}");
                // log.Debug($"{nameof(purchasedProductTransactionId)} {purchasedProductTransactionId}");
                // log.Debug($"{nameof(metadataLocalizedPrice)} {metadataLocalizedPrice}");
                // log.Debug($"{nameof(metadataLocalizedTitle)} {metadataLocalizedTitle}");
                // log.Debug($"{nameof(metadataIsoCurrencyCode)} {metadataIsoCurrencyCode}");
                // log.Debug($"{nameof(metadataLocalizedPriceString)} {metadataLocalizedPriceString}");
            }
            catch (Exception exception)
            {
                UiSoundsManager.Instance().PlayError();
                log.Error(exception.Message);   
            }
        }
    }
}