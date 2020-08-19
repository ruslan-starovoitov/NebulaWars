using System;
using System.Collections.Generic;
using Code.Common;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.Scripts.Shop.Spawners.ItemSpawners
{
    /// <summary>
    /// Создаёт внизу горизонтальную линию из кнопок-указателей.
    /// При нажатии на кнопку отдельный скрипт сместит ScrollView магазина на нужный раздел.
    /// </summary>
    public class FooterPointerUiSpawner:MonoBehaviour
    {
        [SerializeField] private GameObject buttonsParent;
        [SerializeField] private GameObject buttonPointerPrefab;
        private ScrollViewSmoothMovementBehaviour scrollViewSmoothMovement;
        private readonly ILog log = LogManager.CreateLogger(typeof(FooterPointerUiSpawner));
        
        private void Awake()
        {
            scrollViewSmoothMovement = FindObjectOfType<ScrollViewSmoothMovementBehaviour>()
                ?? throw new NullReferenceException(nameof(ScrollViewSmoothMovementBehaviour));
        }

        public void SpawnButtons(Dictionary<string, float> sectionStartPosition)
        {
            //удалить все кнопки    
            ClearPointerButtons();

            foreach( var pair in sectionStartPosition)
            {
                string sectionName = pair.Key;
                SpawnButton(sectionName); 
            }
        }

        private void ClearPointerButtons()
        {
            buttonsParent.transform.DestroyAllChildren();
        }
        
        private void SpawnButton(string sectionName)
        {
            GameObject buttonGo = Instantiate(buttonPointerPrefab, buttonsParent.transform, false);
            
            //установить текст
            Text text = buttonGo.transform.Find("Text").GetComponent<Text>();
            text.text = sectionName;
            
            //установить слушатель
            Button button = buttonGo.transform.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                scrollViewSmoothMovement.StartMovement(sectionName);
            });
        }
    }
}