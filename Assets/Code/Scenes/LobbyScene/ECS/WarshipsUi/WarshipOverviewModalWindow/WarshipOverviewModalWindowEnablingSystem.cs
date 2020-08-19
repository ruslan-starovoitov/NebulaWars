using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Code.Common;
using Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher;
using Code.Scenes.LobbyScene.Scripts.WarshipsUi;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipOverviewModalWindow
{
    public class WarshipOverviewModalWindowEnablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly WarshipsUiStorage warshipsUiStorage;
        private readonly LobbyLayoutSwitcher lobbyLayoutSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipOverviewModalWindowEnablingSystem));
        
        public WarshipOverviewModalWindowEnablingSystem(IContext<LobbyUiEntity> context, 
            WarshipsUiStorage warshipsUiStorage, LobbyLayoutSwitcher lobbyLayoutSwitcher) 
            : base(context)
        {
            this.warshipsUiStorage = warshipsUiStorage;
            this.lobbyLayoutSwitcher = lobbyLayoutSwitcher;
        }
        
        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.EnableWarshipOverviewModalWindow);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasEnableWarshipOverviewModalWindow;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            LobbyUiEntity entity = entities.Last();
            ShowWindow();
            SetCharacteristics(entity.enableWarshipOverviewModalWindow.WarshipDto);
            DisableImproveButton();
            lobbyLayoutSwitcher.SetCurrentLayer(ShittyUiLayerState.WarshipOverviewModalWindow);
        }
        
        private void ShowWindow()
        {
            warshipsUiStorage.modalWindowHeaderText.text = "WARSHIP CHARACTERISTICS";
            warshipsUiStorage.popupWindow.SetActive(true); 
        }

        private void DisableImproveButton()
        {
            warshipsUiStorage.modalWindowImproveButton.SetActive(false);
        }
        
        private void SetCharacteristics(WarshipDto warshipDto)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/LobbyWarshipModalWindow/Empty_Parameter");
            
            //Заполнить раздел защиты
            Transform defenceParent = warshipsUiStorage.defenceHorizontalLayout.transform;
            SetSectionParameters(warshipDto.PowerLevel, warshipDto.WarshipCharacteristics.DefenceParameters, prefab, defenceParent);
            
            //Заполнить раздел атаки
            Transform attackParent = warshipsUiStorage.attackGridLayout.transform;
            SetSectionParameters(warshipDto.PowerLevel, warshipDto.WarshipCharacteristics.AttackParameters, prefab, attackParent);
            
            //Заполнить раздел ульты
            Transform ultimateParent = warshipsUiStorage.ultimateGridLayout.transform;
            SetSectionParameters(warshipDto.PowerLevel, warshipDto.WarshipCharacteristics.UltimateParameters, prefab, ultimateParent);
            
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

                try
                {
                    Text parameterName = go.transform.Find("Text_Name").GetComponent<Text>();
                    parameterName.text = warshipCharacteristic.Name;
                
                    Text parameterValue = go.transform.Find("Text_ParameterValue").GetComponent<Text>();
                    parameterValue.text = warshipCharacteristic.GetCurrentValue(powerLevel).ToString("0.###");
                
                    go.transform.Find("Particle System").gameObject.SetActive(false);
                }
                catch (Exception e)
                {
                    log.Error(e.Message+" "+e.StackTrace);
                }              
            }
        }
    }
}