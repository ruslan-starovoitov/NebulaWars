using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Code.Common;
using Code.Common.Statistics;
using Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher;
using Code.Scenes.LobbyScene.ECS.Warships.Utils;
using Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipOverview.Skins.Utils;
using Code.Scenes.LobbyScene.Scripts;
using Code.Scenes.LobbyScene.Scripts.Shop;
using Code.Scenes.LobbyScene.Scripts.WarshipsUi;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipOverview
{
    public class WarshipOverviewEnablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly TextTooltip textTooltip;
        private readonly LobbyUiContext lobbyUiContext;
        private readonly WarshipsUiStorage warshipsUiStorage;
        private readonly LobbyEcsController lobbyEcsController;
        private readonly LobbyLayoutSwitcher lobbyLayoutSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipOverviewEnablingSystem));

        public WarshipOverviewEnablingSystem(Contexts contexts, WarshipsUiStorage warshipsUiStorage,
           LobbyLayoutSwitcher lobbyLayoutSwitcher, LobbyEcsController lobbyEcsController, TextTooltip textTooltip) 
            : base(contexts.lobbyUi)
        {
            lobbyUiContext = contexts.lobbyUi;
            this.warshipsUiStorage = warshipsUiStorage;
            this.lobbyEcsController = lobbyEcsController;
            this.textTooltip = textTooltip;
            this.lobbyLayoutSwitcher = lobbyLayoutSwitcher;
        }
        
        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.EnableWarshipOverviewUiLayer);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasEnableWarshipOverviewUiLayer;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            LobbyUiEntity entity = entities.Last();
            ShowOverviewLayer(entity.enableWarshipOverviewUiLayer.WarshipDto);
            lobbyLayoutSwitcher.SetCurrentLayer(ShittyUiLayerState.WarshipOverview);
        }
        
        private void ShowOverviewLayer(WarshipDto warshipDto)
        {
            warshipsUiStorage.hint.SetActive(false);
            ShowWarshipOverview(warshipDto);
            warshipsUiStorage.warshipListRootGameObject.SetActive(false);
            warshipsUiStorage.warshipRootGameObject.SetActive(true);
            warshipsUiStorage.warshipRoot.SetActive(true);
        }
        
        private void ShowWarshipOverview(WarshipDto warshipDto)
        {
            UpdateData(warshipDto);
            
            //Обновить уникальный компонент для листания и переключения скинов
            lobbyUiContext.ReplaceWarshipOverviewCurrentSkinModel( warshipDto.CurrentSkinIndex, warshipDto);
        }
        
        private void UpdateData(WarshipDto warshipDto)
        {
            //Установить название корабля
            warshipsUiStorage.warshipName.text = warshipDto.WarshipName.ToUpper();
            //Установить тип корабля
            warshipsUiStorage.warshipTypeName.text = warshipDto.CombatRoleName;
            //Установить ранг корабля
            WarshipRankModel rankModel = WarshipRatingScaleStorage.Instance.GetRankModel(warshipDto.Rating);
            warshipsUiStorage.warshipRank.text = rankModel.currentRank.ToString();
            //Установить слайдер для рейтинга
            warshipsUiStorage.trophySlider.value = rankModel.rankProgress;
            //Установить рейтинг корабля
            warshipsUiStorage.trophyText.text = rankModel.ToString();
            //Установить описание корабля
            warshipsUiStorage.warshipDescription.text = warshipDto.Description;
            //Установить уровень силы
            warshipsUiStorage.warshipPowerLevel.text = "POWER " + warshipDto.PowerLevel;
            //TODO Установить характеристики корабля (атаку, защиту, скорость или что там)

            // log.Debug($"attack name "+warshipDto.WarshipCharacteristics.AttackName);
            // log.Debug($"ultimate name "+warshipDto.WarshipCharacteristics.UltimateName);
            
            var healthParameter = warshipDto.WarshipCharacteristics.DefenceParameters
                .SingleOrDefault(p => p.Name == "Health");
            warshipsUiStorage.healthText.text = healthParameter!=null?healthParameter.GetCurrentValue(warshipDto.PowerLevel).ToString("0.###") :"undefined";
            var movementSpeed = warshipDto.WarshipCharacteristics.DefenceParameters
                .SingleOrDefault(p => p.Name == "Movement speed");
            warshipsUiStorage.velocityText.text = movementSpeed!=null?movementSpeed.GetCurrentValue(warshipDto.PowerLevel).ToString("0.###") :"undefined";
            warshipsUiStorage.attackNameText.text = warshipDto.WarshipCharacteristics.AttackName;
            warshipsUiStorage.ultimateNameText.text = warshipDto.WarshipCharacteristics.UltimateName;
            
            //Проверить на кол-во ресурсов для перехода на новый уровень
            int softCurrency = lobbyEcsController.GetSoftCurrency();
            var model = WarshipPowerScale.GetModel(warshipDto.PowerLevel);
            int improvementCost = model.SoftCurrencyCost;
            int maxPowerPoints = model.PowerPointsCost;
                
            bool showGreenScale = softCurrency >= improvementCost && warshipDto.PowerPoints >= maxPowerPoints;

            Text powerPointsValueText;
            //Показать нужную шкалу. (Красную или зелёную)
            if (showGreenScale)
            {
                powerPointsValueText = warshipsUiStorage.greenPowerPointsValueText;
                warshipsUiStorage.redScale.SetActive(false);
                warshipsUiStorage.greenScale.SetActive(true);
            }
            else
            {
                warshipsUiStorage.redScale.SetActive(true);
                warshipsUiStorage.greenScale.SetActive(false);
                powerPointsValueText = warshipsUiStorage.powerValueText;
            }

            //Установить кол-во очков силы для текущего уровня
            powerPointsValueText.text =  warshipDto.PowerPoints + "/" + maxPowerPoints;

            //Установить слайдер для кол-ва очков силы
            warshipsUiStorage.powerSlider.maxValue = maxPowerPoints;
            warshipsUiStorage.powerSlider.value = warshipDto.PowerPoints;
            
            //Установить цену улучшения
            warshipsUiStorage.improveButtonCost.text = improvementCost.ToString();
            //Установить слушатель для кнопки улучшения
            warshipsUiStorage.improveButton.onClick.RemoveAllListeners();
            warshipsUiStorage.improveButton.onClick.AddListener(() =>
            {
                if (warshipDto.PowerPoints < maxPowerPoints)
                {
                    UiSoundsManager.Instance().PlayError();
                    string message = "The warship doesn't have enough power points.";
                    textTooltip.Show(message);
                }
                else if (softCurrency< improvementCost)
                {
                    UiSoundsManager.Instance().PlayError();
                    string message = "There's not enough coins to improve the spaceship.";
                    textTooltip.Show(message);
                }
                else
                {
                    UiSoundsManager.Instance().PlayClick();
                    //показать окно покупки улучшения
                    lobbyEcsController.ShowWarshipImprovementModalWindow(warshipDto);
                }
            });

            string oldSkinName = warshipDto.GetCurrentSkinName();
            //Установить слушатель для кнопки выбора корабля   
            warshipsUiStorage.chooseButton.onClick.RemoveAllListeners();
            warshipsUiStorage.chooseButton.onClick.AddListener(() =>
            {
                CurrentWarshipTypeStorage.WriteWarshipType(warshipDto.WarshipTypeEnum);
                // log.Debug("Слушатель работает");
                UiSoundsManager.Instance().PlayClick();
                //заменить скин если нужно
                int actualSkinIndex = lobbyUiContext.warshipOverviewCurrentSkinModel.skinIndex;
                string newSkinName = warshipDto.GetCurrentSkinName();
                if (oldSkinName != newSkinName)
                {
                    warshipDto.CurrentSkinIndex = actualSkinIndex;
                    int warshipId = warshipDto.Id;
                    var task = new SkinChangingNotifier().ChangeSkinOnServerAsync(warshipId, newSkinName);
                }
                else
                {
                    log.Info("Скин не был изменён");
                }
                //изменить тип текущего корабля
                
                lobbyUiContext.ReplaceCurrentWarshipTypeEnum(warshipDto.WarshipTypeEnum);
                //заменть компонент корабля
                lobbyUiContext.CreateEntity().AddWarship(warshipDto);
                //выключить меню обзора корабля
                lobbyUiContext.CreateEntity().messageDisableWarshipOverviewUiLayer = true;
                //выключить меню со списком кораблей
                lobbyUiContext.CreateEntity().messageDisableWarshipListUiLayer = true;
            });
            
            //Установить стоимость для кнопки покупки улучшения
            warshipsUiStorage.popupWindowCostText.text = improvementCost.ToString();
            
            //Установить слушатель для кнопки покупки улучшения
            warshipsUiStorage.popupWindowBuyButton.onClick.RemoveAllListeners();
            warshipsUiStorage.popupWindowBuyButton.onClick.AddListener(()=>
            {
                //todo показать окно подтверждения
            });
            
            //Установить слушатель для меню с характеристиками корабля
            warshipsUiStorage.warshipCharacteristicsButton.onClick.RemoveAllListeners();
            warshipsUiStorage.warshipCharacteristicsButton.onClick.AddListener(()=>
            {
                if (warshipDto.PowerPoints < maxPowerPoints||softCurrency< improvementCost)
                {
                    UiSoundsManager.Instance().PlayClick();
                    lobbyEcsController.ShowWarshipCharacteristics(warshipDto);
                }
                else
                {
                    UiSoundsManager.Instance().PlayClick();
                    //показать окно покупки улучшения
                    lobbyEcsController.ShowWarshipImprovementModalWindow(warshipDto);
                }
            });
        }
    }
}