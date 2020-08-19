using System.Collections.Generic;
using System.Linq;
using NetworkLibrary.NetworkLibrary.Http;
using ZeroFormatter;
using ShopModel = Libraries.NetworkLibrary.Http.Lobby.Shop.ShopModel;

namespace Code.Scenes.LobbyScene.Scripts.Shop
{
    public static class ShopModelExtensions
    {
        public static List<ProductModel> GetAllProducts(this ShopModel shopModel)
        {
            List<ProductModel> productModels = shopModel.UiSections
                .SelectMany(container => container.UiItems)
                .SelectMany(test1=>test1)
                .ToList();
            return productModels;
        }
        
        public static List<RealCurrencyCostModel> GetRealCurrencyProducts(this ShopModel shopModel)
        {
            List<RealCurrencyCostModel> realCurrencyProducts =  shopModel.UiSections
                .SelectMany(container => container.UiItems)
                .SelectMany(test1=>test1)
                .Where(productModel => productModel.CostModel.CostTypeEnum == CostTypeEnum.RealCurrency)
                .Select(productModel => ZeroFormatterSerializer.Deserialize<RealCurrencyCostModel>(productModel.CostModel.SerializedCostModel))
                .ToList();
            return realCurrencyProducts;
        }
    }
}