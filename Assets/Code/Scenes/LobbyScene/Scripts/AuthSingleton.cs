#define FORCE_AUTH

using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using Code.Scenes.LobbyScene.Scripts.Shop;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
namespace Code.Scenes.LobbyScene.Scripts
{
   /// <summary>
   /// Отвечает за авторизацию. Остаётся на сцене всегда.
   /// </summary>
   public class AuthSingleton : Singleton<AuthSingleton>
   {
      private bool isAuthorizationCompleted;
      private const string UsernameKey = "username";
      private readonly object lockObj = new object();
      protected override bool DontDestroy { get; } = true;
      private readonly ILog log = LogManager.CreateLogger(typeof(AuthSingleton));

      private void Start()
      {
#if UNITY_ANDROID
         if (!PlayGamesPlatform.Instance.IsAuthenticated())
         {
            log.Info("Игрок ещё не зашёл в аккаунт");
            StartAuth();
         }
         else
         {
            log.Debug("Игрок уже зашёл в аккаунт");
            isAuthorizationCompleted = true;
            PrintPlayerData();
            PlayerIdStorage.SetServiceId(Social.localUser.id);
         }
#else
          StartAuth();
#endif
      }

      private void StartAuth()
      {
#if UNITY_ANDROID
         PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false /*forceRefresh*/)
            .Build();
         PlayGamesPlatform.InitializeInstance(config);
         PlayGamesPlatform.Activate();
         log.Info("Play Games Configuration initialized");
#endif
            Social.localUser.Authenticate(AuthCallback);
      }
      
      private void AuthCallback(bool success)
      {
         log.Info($"{nameof(AuthCallback)} {nameof(success)} {success}");
         if (success)
         {
            log.Info("Игрок авторизован");
            PrintPlayerData();
            lock (lockObj)
            {
               PlayerIdStorage.SetServiceId(Social.localUser.id);
               PlayerIdStorage.SetUsername(Social.localUser.userName);
               isAuthorizationCompleted = true;
            }
         }
         else
         {
            log.Error("Auth failed");
#if (UNITY_EDITOR || FORCE_AUTH)
            AuthIfDeveloper();
#endif
         }
      }

#if (UNITY_EDITOR || FORCE_AUTH)
      private void AuthIfDeveloper()
      {
         log.Warn("Принудительное выставление флага успеха авторизации");
         const string playerServiceIdKey = "playerId";

         //Прочитать из жесткого диска
         string playerServiceId = PlayerPrefs.GetString(playerServiceIdKey);
         string username = PlayerPrefs.GetString(UsernameKey);
         
         if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(playerServiceId))
         {
            var usernameMenuController = FindObjectOfType<UsernameMenuController>();
            usernameMenuController.ShowMenu();
            //Создать новый serviceId
            playerServiceId = $"playerServiceId_{new System.Random().Next(1, int.MaxValue)}";
            username = $"username {new System.Random().Next(1, ushort.MaxValue)}";
            //Сохранить на  диск
            PlayerPrefs.SetString(playerServiceIdKey, playerServiceId);
            PlayerPrefs.SetString(UsernameKey, username);
            
         }

         //Присвоить serviceId
         lock (lockObj)
         {
            PlayerIdStorage.SetServiceId(playerServiceId);
            PlayerIdStorage.SetUsername(username);
            isAuthorizationCompleted = true;
         }
      }
#endif

      public async Task<UsernameValidationResultEnum> TrySetUsernameAsync(string newUsername)
      {
         log.Debug(Thread.CurrentThread.Name);
         UsernameChangingService service = new UsernameChangingService();
         var result = await service.ChangesUsernameAsync(newUsername);
         log.Debug(Thread.CurrentThread.Name);
         if (result == UsernameValidationResultEnum.Ok)
         {
            UnityThread.Execute(() =>
            {
               log.Debug(Thread.CurrentThread.Name);
               PlayerPrefs.SetString(UsernameKey, newUsername);
               PlayerIdStorage.SetUsername(newUsername);
               var lobbyEcsController = FindObjectOfType<LobbyEcsController>();
               lobbyEcsController.ReplaceUsername(newUsername);
            });
         }

         return result;
      }

      private void PrintPlayerData()
      {
         log.Info(nameof(PrintPlayerData));
         log.Info($"{nameof(Social.localUser.authenticated)} {Social.localUser.authenticated}");
         log.Info($"{nameof(Social.localUser.underage)} {Social.localUser.underage}");
         log.Info($"{nameof(Social.localUser.id)} {Social.localUser.id}");
         log.Info($"{nameof(Social.localUser.state)} {Social.localUser.state}");
         log.Info($"{nameof(Social.localUser.userName)} {Social.localUser.userName}");
      }

      /// <summary>
      /// Нужно для того, чтобы понять можно ли выключать скринсейвер.
      /// </summary>
      /// <returns></returns>
      public bool IsAuthorizationCompleted()
      {
         lock (lockObj)
         {
            return isAuthorizationCompleted;
         }
      }
   }
}
