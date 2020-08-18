using System;
using Code.Common.Logger;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.Scripts.AccountModel
{
    /// <summary>
    /// Если версия сборки не последняя, то перенаправит в PlayMarket для обновления
    /// </summary>
    public class UpdateGameMenuSwitcher:MonoBehaviour
    {
        [SerializeField] public Button updateGame;
        [SerializeField] public GameObject updateGameMenuRoot;
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateGameMenuSwitcher));
        
        private void Awake()
        {
            if (updateGameMenuRoot == null)
            {
                throw new NullReferenceException(nameof(updateGameMenuRoot));
            }
            updateGameMenuRoot.SetActive(false);
            updateGame.onClick.RemoveAllListeners();
            updateGame.onClick.AddListener(UpdateGameButton_OnClick);
        }

        private void UpdateGameButton_OnClick()
        {
            const string packageName = "com.tikaytech.nebulaWars2";
            Application.OpenURL("market://details?id="+packageName);
        }

        public void CheckBundleVersion(string actualBundleVersionFromServer)
        {
            log.Info($"версия "+actualBundleVersionFromServer);
            
            string currentBundleVersion = Application.version;
            if (currentBundleVersion != actualBundleVersionFromServer)
            {
                log.Error($"Версия игры не совпадает. " +
                         $"currentBundleVersion  {currentBundleVersion}" +
                         $"actualBundleVersionFromServer {actualBundleVersionFromServer}");
                updateGameMenuRoot.SetActive(true);
            }
            else
            {
                log.Info("Актуальная версия игры "+currentBundleVersion);
            }
        }
    }
}