using System.Collections;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.Screensaver
{
    /// <summary>
    /// Отвечает за выключение заставки перед лобби.
    /// </summary>
    public class ScreenSaverSwitcher : MonoBehaviour
    {
        // private AuthSingleton authSingleton;
        private LobbyEcsController lobbyEcsController;
        [SerializeField] private GameObject screenSaverImage;
        private readonly ILog log = LogManager.CreateLogger(typeof(ScreenSaverSwitcher));
        
        private float _nextLogTime;
        private bool _isAuthCompleted;
        private bool _isWarshipsCompleted;
        
        private void Awake()
        {
            // authSingleton = FindObjectOfType<AuthSingleton>();
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
        }

        private void Start()
        {
            screenSaverImage.SetActive(true);
            StartCoroutine(DisableScreenSaverWhenInitializationIsCompleted());
        }

        private IEnumerator DisableScreenSaverWhenInitializationIsCompleted()
        {
            log.Info(nameof(DisableScreenSaverWhenInitializationIsCompleted)+" start");
            
            while (!IsInitializationCompleted())
            {
                yield return null;
            }
            
            log.Info("Инициализация закончена. Выключение скринсейвера");
            screenSaverImage.SetActive(false);
        }
        
        private bool IsInitializationCompleted()
        {
            // bool isAuthorizationCompleted = authSingleton.IsAuthorizationCompleted();
            bool isAuthorizationCompleted = true;
            bool warshipsCreationCompleted = lobbyEcsController.IsWarshipsCreationCompleted();
            bool isUnityEditor = IsUnityEditor();
            bool initializationCompleted = isAuthorizationCompleted && warshipsCreationCompleted;
            bool result = initializationCompleted || isUnityEditor;

            float currentTime = Time.time;
            if (_nextLogTime < currentTime)
            {
                log.Info($"time {nameof(isAuthorizationCompleted)} {isAuthorizationCompleted}" +
                         $" {nameof(warshipsCreationCompleted)} {warshipsCreationCompleted}");
                _nextLogTime += currentTime+1;
            }

            if (_isAuthCompleted != isAuthorizationCompleted)
            {
                log.Info($"_isAuthCompleted changed {nameof(isAuthorizationCompleted)} {isAuthorizationCompleted}" +
                         $" {nameof(warshipsCreationCompleted)} {warshipsCreationCompleted}");
                _isAuthCompleted = isAuthorizationCompleted;
            }
            
            if (_isWarshipsCompleted != warshipsCreationCompleted)
            {
                log.Info($"_isWarshipsCompleted changed {nameof(isAuthorizationCompleted)} {isAuthorizationCompleted}" +
                         $" {nameof(warshipsCreationCompleted)} {warshipsCreationCompleted}");
                _isWarshipsCompleted = warshipsCreationCompleted;
            }
            
            return result;
        }

        private bool IsUnityEditor()
        {
#if UNITY_EDITOR
    return true;
#else
    return false;
#endif
        }
    }
};