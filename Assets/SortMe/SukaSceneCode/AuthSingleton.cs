// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using UnityEngine;
//
// namespace Code.Scenes.SukaScene
// {
//    /// <summary>
//    /// Отвечает за авторизацию. Остаётся на сцене всегда.
//    /// </summary>
//    public class AuthSingleton : MonoBehaviour
//    {
//       private void Start()
//       {
//          if (!PlayGamesPlatform.Instance.IsAuthenticated())
//          {
//             Debug.Log("Игрок ещё не зашёл в аккаунт");
//             StartAuth();
//          }
//          else
//          {
//             Debug.Log("Игрок уже зашёл в аккаунт");
//             PrintPlayerData();
//          }
//       }
//
//       private void StartAuth()
//       {
//          PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
//             .RequestServerAuthCode(false /*forceRefresh*/)
//             .Build();
//          PlayGamesPlatform.InitializeInstance(config);
//          PlayGamesPlatform.Activate();
//          Debug.Log("Play Games Configuration initialized");
//
//          Social.localUser.Authenticate(AuthCallback);
//       }
//       
//       private void AuthCallback(bool success)
//       {
//          Debug.Log($"{nameof(AuthCallback)} {nameof(success)} {success}");
//          if (success)
//          {
//             Debug.Log("Игрок авторизован");
//             PrintPlayerData();
//          
//          }
//          else
//          {
//             Debug.Log("Auth failed");
//          }
//       }
//
//       private void PrintPlayerData()
//       {
//          Debug.Log(nameof(PrintPlayerData));
//          Debug.Log($"{nameof(Social.localUser.authenticated)} {Social.localUser.authenticated}");
//          Debug.Log($"{nameof(Social.localUser.underage)} {Social.localUser.underage}");
//          Debug.Log($"{nameof(Social.localUser.id)} {Social.localUser.id}");
//          Debug.Log($"{nameof(Social.localUser.state)} {Social.localUser.state}");
//          Debug.Log($"{nameof(Social.localUser.userName)} {Social.localUser.userName}");
//       }
//    }
// }