using System;
using System.Collections.Generic;
using Code.Common;
using Code.Scenes.LobbyScene.Scripts.Shop.Spawners.ItemSpawners;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.Shop.Spawners
{
    /// <summary>
    /// Заполняет меню магазина после получения данных от сервера.
    /// </summary>
    public class ShopUiSpawner : MonoBehaviour
    {
        private GameObject sectionsParentGo;
        private SectionSpawner sectionSpawner;
        private FooterPointerUiSpawner sectionPointerUiSpawner;
        private ScrollViewSmoothMovementBehaviour scrollViewSmoothMovement;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShopUiSpawner));

        private void Awake()
        {
            ShopUiStorage shopUiStorageScript = FindObjectOfType<ShopUiStorage>()
                            ?? throw new NullReferenceException(nameof(ShopUiStorage));
            sectionSpawner = FindObjectOfType<SectionSpawner>()
                            ?? throw new NullReferenceException(nameof(SectionSpawner));
            sectionPointerUiSpawner = FindObjectOfType<FooterPointerUiSpawner>()
                            ?? throw new NullReferenceException(nameof(FooterPointerUiSpawner));
            scrollViewSmoothMovement = FindObjectOfType<ScrollViewSmoothMovementBehaviour>()
                            ?? throw new NullReferenceException(nameof(ScrollViewSmoothMovementBehaviour));
            sectionsParentGo = shopUiStorageScript.shopSectionsParent;
        }

        public void Spawn(ShopModel shopModel)
        {
            ClearShop();
            
            //Создать разделы
            foreach (SectionModel shopSectionModel in shopModel.UiSections)
            {
                sectionSpawner.SpawnSection(shopSectionModel, shopModel.Id);
            }

            var requiredSectionNames = shopModel.GetRequiredSectionNames();
            scrollViewSmoothMovement.InitRequiredSections(requiredSectionNames);
        }

        public void SpawnFooterPointers()
        {
            //название раздела + координата левого края раздела
            Dictionary<string, float> sectionStartPositions = new Dictionary<string, float>();
            
            //Сохранить координаты интересующих разделов
            foreach (Transform section in sectionsParentGo.transform)
            {
                //Если нет (Clone), то к этому разделу будет нужно перелистывать
                if (!section.gameObject.name.Contains("(Clone)"))
                {
                    RectTransform rect = section.gameObject.GetComponent<RectTransform>();
                    float sectionPosition = rect.localPosition.x;
                    // log.Debug("sectionPosition = "+sectionPosition);
                    sectionStartPositions.Add(section.name, sectionPosition);    
                }
            }
            
            
            //иниуиализировать скрипт для листания разделов
            scrollViewSmoothMovement.Initialize(sectionStartPositions);
            
            //создать кнопки-указатели
            sectionPointerUiSpawner.SpawnButtons(sectionStartPositions);
        }
        
        /// <summary>
        /// Очищает содержимое scrollView от тестовых разделов.
        /// </summary>
        private void ClearShop()
        {
            sectionsParentGo.transform.DestroyAllChildren();
        }
    }
}