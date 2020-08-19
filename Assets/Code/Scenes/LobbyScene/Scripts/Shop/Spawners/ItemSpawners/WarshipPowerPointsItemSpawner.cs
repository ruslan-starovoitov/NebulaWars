using System.Collections.Generic;
using System.Globalization;
using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;
using ZeroFormatter;

namespace Code.Scenes.LobbyScene.Scripts.Shop.Spawners.ItemSpawners
{
    public class StubWppUi
    {
        public GameObject darkLayer;
        public GameObject costText;
        public GameObject boughtText;
    }
    public class WarshipPowerPointsItemSpawner
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipPowerPointsItemSpawner));
        //todo говнокод
        private readonly Dictionary<int, StubWppUi> dictionary = new Dictionary<int, StubWppUi>();

        public void Disable(int productId)
        {
            //todo говнокод
            if (dictionary.TryGetValue(productId, out var wppUi))
            {
                wppUi.boughtText.SetActive(true);
                wppUi.costText.SetActive(false);
                wppUi.darkLayer.SetActive(true);
            }
        }
        
        public void Spawn(PurchaseModel purchaseModel, GameObject sectionGameObject,
            ProductClickHandlerScript productClickHandlerScript)
        {
            WarshipPowerPointsProductModel model = purchaseModel.productModel;
            int increment = model.Increment;
            InGameCurrencyCostModel costModel =
                ZeroFormatterSerializer.Deserialize<InGameCurrencyCostModel>(purchaseModel.productModel.CostModel
                    .SerializedCostModel);
            //Создать объект на сцене
            GameObject wppPrefab = Resources.Load<GameObject>("Prefabs/LobbyShop/Image_WarshipPowerPointsItem");
            GameObject wppGo = Object.Instantiate(wppPrefab, sectionGameObject.transform, false);

            //Установить название обьекта
            wppGo.name += " " + increment;

            //Заполнить картинку
            Image itemPreview = wppGo.transform.Find("Image_WarshipPreview")
                .GetComponentInChildren<Image>();
            itemPreview.sprite = Resources.Load<Sprite>("SkinPreview/" + purchaseModel.productModel.PreviewImagePath);

            //Заполнить текущий показатель силы
            Text currentPowerValue = wppGo.transform.Find("Empty_PowerValueRoot/Text").GetComponent<Text>();
            currentPowerValue.text = $"{model.SupportClientModel.StartValue}/{model.SupportClientModel.MaxValueForLevel}";

            //Установить значение слайдера
            Slider slider = wppGo.transform.Find("Empty_PowerValueRoot/Slider").GetComponent<Slider>();
            slider.value = 1f * model.SupportClientModel.StartValue / model.SupportClientModel.MaxValueForLevel;

            //Заполнить прибавку к очкам силы
            Text incrementText = wppGo.transform.Find("Text").GetComponent<Text>();
            // log.Debug($"increment = "+increment);
            incrementText.text = $"+{increment}";

            //Заполнить цену
            Text itemCost = wppGo.transform.Find("Image_Cost/Text_Amount").GetComponent<Text>();
            itemCost.text = costModel.Cost.ToString(CultureInfo.InvariantCulture);

            var darkLayer = wppGo.transform.Find("Image_Disabled").gameObject;
            darkLayer.SetActive(purchaseModel.productModel.IsDisabled);
            var boughtText = wppGo.transform.Find("Image_Cost/Text_Bought").gameObject;
            boughtText.SetActive(purchaseModel.productModel.IsDisabled);
            var costText = wppGo.transform.Find("Image_Cost/Text_Amount").gameObject;
            costText.SetActive(!purchaseModel.productModel.IsDisabled);

            if (dictionary.ContainsKey(purchaseModel.productModel.Id))
            {
                dictionary.Remove(purchaseModel.productModel.Id);
            }
            dictionary.Add(purchaseModel.productModel.Id, new StubWppUi()
            {
                boughtText = boughtText,
                costText = costText,
                darkLayer = darkLayer
            });

            //Установить обработчик нажатия
            Button itemButton = wppGo.GetComponent<Button>();
            itemButton.onClick.RemoveAllListeners();
            
            itemButton.onClick.AddListener(() =>
            {
                productClickHandlerScript.Product_OnClick(purchaseModel);
            });
        }
    }
}