using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Code.Common.NetworkStatistics;
using Code.Common.Statistics;
using Code.Scenes.LobbyScene.Utils;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.AccountModel
{
    /// <summary>
    /// Упраляет обновлением всех данных аккаунта при старте сцены.
    /// </summary>
    public class LobbyModelLoadingInitiator : MonoBehaviour
    {
        private CancellationTokenSource cts;
        private LobbyEcsController lobbyEcsController;
        private UpdateGameMenuSwitcher updateGameVersionMenuSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(LobbyModelLoadingInitiator));

        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
            updateGameVersionMenuSwitcher = FindObjectOfType<UpdateGameMenuSwitcher>();
        }

        private void Start()
        {
            NetworkStatisticsStorage.Instance.PrintSavedMatches();
            StartCoroutine(UpdateAccountData());
        }

        private IEnumerator UpdateAccountData()
        {
            cts = new CancellationTokenSource();
            Task<LobbyModel> task = new LobbyModelLoader().Load(cts.Token);
            yield return new WaitUntil(()=>task.IsCompleted);
            if (task.IsFaulted||task.IsCanceled)
            {
                log.Fatal("Не удалось скачать модель аккаунта");
                yield break;
            }
            
            SetData(task.Result);
        }

        private void SetData(LobbyModel lobbyModel)
        {
            //Заблокировать ui, если версия игры старая
            updateGameVersionMenuSwitcher.CheckBundleVersion(lobbyModel.BundleVersion);
            
            //Отнять от данных аккаунта значения, которые будут начислены с анимацией
            lobbyEcsController.SetLobbyModel(lobbyModel);
            
            //Установить данные для шкалы рейтинга кораблей
            WarshipRatingScaleStorage.Instance.SetValue(lobbyModel.WarshipRatingScaleModel);
        }

        private void OnDestroy()
        {
            cts?.Cancel();
        }
    }
}