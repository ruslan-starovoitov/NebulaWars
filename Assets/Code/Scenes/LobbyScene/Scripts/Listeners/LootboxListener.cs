using System;
using Code.Scenes.LobbyScene.Scripts.ResourcesAccrual;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.Listeners
{
    /// <summary>
    /// Отвечает за обработку нажатия на лутбокс.
    /// </summary>
    public class LootboxListener : MonoBehaviour
    {
        private LobbyEcsController lobbyEcsController;
        private DateTime nextClickTime = DateTime.UtcNow;
        private ResourcesAccrualSceneManager resourcesAccrualSceneManager;
        private readonly ILog log = LogManager.CreateLogger(typeof(LootboxListener));
        
        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
            resourcesAccrualSceneManager = FindObjectOfType<ResourcesAccrualSceneManager>();
        }

        public void OpenLootboxButton_OnClick()
        {
            if (nextClickTime > DateTime.UtcNow)
            {
                return;
            }

            nextClickTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(500);
            if (!lobbyEcsController.LootboxCanBeOpened())
            {
                log.Info("Недостаточно баллов для открытия лутбокса");
                return;
            }
            
            resourcesAccrualSceneManager.ShowLootboxScene();
        }
    }
}