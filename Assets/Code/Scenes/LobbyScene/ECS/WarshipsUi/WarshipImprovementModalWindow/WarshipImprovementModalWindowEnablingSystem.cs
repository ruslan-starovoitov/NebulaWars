using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Common;
using Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher;
using Code.Scenes.LobbyScene.Scripts;
using Code.Scenes.LobbyScene.Scripts.WarshipsUi;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipImprovementModalWindow
{
    public class WarshipImprovementModalWindowEnablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly WarshipsUiStorage warshipsUiStorage;
        private readonly LobbySceneSwitcher lobbySceneSwitcher;
        private readonly LobbyLayoutSwitcher lobbyLayoutSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipImprovementModalWindowEnablingSystem));
        
        public WarshipImprovementModalWindowEnablingSystem(IContext<LobbyUiEntity> context, 
            WarshipsUiStorage warshipsUiStorage, LobbyLayoutSwitcher lobbyLayoutSwitcher, 
            LobbySceneSwitcher lobbySceneSwitcher) 
            : base(context)
        {
            this.warshipsUiStorage = warshipsUiStorage;
            this.lobbyLayoutSwitcher = lobbyLayoutSwitcher;
            this.lobbySceneSwitcher = lobbySceneSwitcher;
        }
        
        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.EnableWarshipImprovementModalWindow);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasEnableWarshipImprovementModalWindow;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            LobbyUiEntity entity = entities.Last();
            ShowWindow();
            SetCharacteristics(entity.enableWarshipImprovementModalWindow.WarshipDto);
            EnableImproveButton();
            AddListenerToImproveButton(entity.enableWarshipImprovementModalWindow.WarshipDto);
            lobbyLayoutSwitcher.SetCurrentLayer(ShittyUiLayerState.WarshipImprovementModalWindow);
        }

        private void AddListenerToImproveButton(WarshipDto warshipDto)
        {
            var button = warshipsUiStorage.modalWindowImproveButton.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                UnityThread.ExecuteCoroutine(Coroutine());
            });
            
            IEnumerator Coroutine()
            {
                //Отправить запрос
                var downloader = new WarshipImprovementDownloader().TryBuy(warshipDto.Id);
                yield return new WaitUntil(()=>downloader.IsCompleted);
                bool success = downloader.Result;
                if (success)
                {
                    UiSoundsManager.Instance().PlayLevelUp();
                    //Показать анимацию получения нового уровня
                    log.Info("Успешное получение уровня");
                    //Перезагрузить сцену
                    lobbySceneSwitcher.ReloadScene();
                    var newLobbyEcs = Object.FindObjectOfType<LobbyEcsController>();
                    yield return new WaitUntil(()=>newLobbyEcs.IsWarshipsCreationCompleted());
                    yield return new WaitUntil(()=>newLobbyEcs.IsSoftCurrencyReady());
                    //Показать текущее меню
                    newLobbyEcs.ShowWarshipList();
                    
                    newLobbyEcs.ShowWarshipOverviewById(warshipDto.Id);
                }
                else
                {
                    UiSoundsManager.Instance().PlayError();
                    //Показать уведомление, что ресурсов недостаточно
                    log.Error("Не удалось получить новый уровень");
                }
            }
        }

        private void ShowWindow()
        {
            warshipsUiStorage.modalWindowHeaderText.text = "RAISE THE POWER LEVEL?";
            warshipsUiStorage.popupWindow.SetActive(true); 
        }

        private void EnableImproveButton()
        {
            warshipsUiStorage.modalWindowImproveButton.SetActive(true);
        }
        
        private void SetCharacteristics(WarshipDto warshipDto)
        {
            GameObject parameterPrefab = Resources
                .Load<GameObject>("Prefabs/LobbyWarshipModalWindow/Empty_Parameter");

            Transform defenceParent = warshipsUiStorage.defenceHorizontalLayout.transform;
            SetSectionParameters(warshipDto.PowerLevel, warshipDto.WarshipCharacteristics.DefenceParameters,
                parameterPrefab, defenceParent);
            
            Transform attackParent = warshipsUiStorage.attackGridLayout.transform;
            SetSectionParameters(warshipDto.PowerLevel, warshipDto.WarshipCharacteristics.AttackParameters,
                parameterPrefab, attackParent);
            
            Transform ultimateParent = warshipsUiStorage.ultimateGridLayout.transform;
            SetSectionParameters(warshipDto.PowerLevel, warshipDto.WarshipCharacteristics.UltimateParameters,
                parameterPrefab, ultimateParent);

            warshipsUiStorage.attackDescription.text = warshipDto.WarshipCharacteristics.AttackDescription;
            warshipsUiStorage.ultimateDescription.text = warshipDto.WarshipCharacteristics.UltimateDescription;

            warshipsUiStorage.attackName.text = warshipDto.WarshipCharacteristics.AttackName;
            warshipsUiStorage.ultimateName.text = warshipDto.WarshipCharacteristics.UltimateName;
        }

        private void SetSectionParameters(int powerLevel, WarshipParameter[] warshipParameters, 
            GameObject parameterPrefab, Transform parent)
        {
            parent.DestroyAllChildren();
            foreach (WarshipParameter warshipCharacteristic in warshipParameters)
            {
                GameObject go = Object.Instantiate(parameterPrefab, parent, false);
                Text parameterName = go.transform.Find("Text_Name").GetComponent<Text>();
                parameterName.text = warshipCharacteristic.Name;
                Text parameterValue = go.transform.Find("Text_ParameterValue").GetComponent<Text>();

                var currentValue = warshipCharacteristic.GetCurrentValue(powerLevel);
                if (warshipCharacteristic.UiIncrementTypeEnum != UiIncrementTypeEnum.None
                    && warshipCharacteristic.Increment != IncrementCoefficient.None)
                {
                    go.transform.Find("Particle System").gameObject.SetActive(true);
                    var incrementValue = warshipCharacteristic.GetCurrentValue(powerLevel + 1) - currentValue;
                    parameterValue.text = $"{currentValue:0.###} <color=#00CE00>+ {incrementValue:0.###}</color>";
                }
                else
                {
                    go.transform.Find("Particle System").gameObject.SetActive(false);
                    parameterValue.text = currentValue.ToString("0.###");
                }
            }
        }
    }
}