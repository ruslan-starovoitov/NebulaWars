using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Logger;
using Code.Common.NetworkStatistics;
using Code.Common.Storages;
using Code.Scenes.BattleScene.Experimental;
using Code.Scenes.BattleScene.Scripts;
using Code.Scenes.LobbyScene.Utils;
using NetworkLibrary.NetworkLibrary.Http;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.MatchLoading
{
    /// <summary>
    /// Отвечает за получение данных о матче и загрузку сцены боя.
    /// </summary>
    public class MatchLoader : MonoBehaviour
    {
        private LobbySceneSwitcher lobbySceneSwitcher;
        private LobbyEcsController lobbyEcsController;
        private CancellationTokenSource battleLoadingCts;
        private MatchmakerNegotiator matchmakerNegotiator;
        private readonly ILog log = LogManager.CreateLogger(typeof(LobbyLoaderController));

        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>()
                                 ?? throw new Exception($"Не найден {nameof(LobbyEcsController)}");
            lobbySceneSwitcher = FindObjectOfType<LobbySceneSwitcher>();
            matchmakerNegotiator = new MatchmakerNegotiator(lobbyEcsController);
        }

        private void OnDestroy()
        {
            battleLoadingCts?.Cancel();
        }

        public void StartBattleLoading(string playerId, int currentWarshipId)
        {
            battleLoadingCts = new CancellationTokenSource();
            StartCoroutine(GetMatchDataAndLoadScene(battleLoadingCts.Token, playerId, currentWarshipId));
        }
        
        public void StopBattleLoading()
        {
            battleLoadingCts.Cancel();
            matchmakerNegotiator.DeleteFromQueueAsync();
        }

        private IEnumerator GetMatchDataAndLoadScene(CancellationToken cancellationToken, string playerId,
            int currentWarshipId)
        {
            log.Info("Поиск матча...");
            
            Task<BattleRoyaleClientMatchModel> task = matchmakerNegotiator.GetMatchDataAsync(cancellationToken, playerId, currentWarshipId);
            yield return new WaitUntil(()=>task.IsCompleted);

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }
            
            BattleRoyaleClientMatchModel matchModel = task.Result;
            if (task.Result != null)
            {
                LoadBattleScene(matchModel, playerId);
            }
            else
            {
                log.Error($"{nameof(matchModel)} was null");
            }
        }
        
        private void LoadBattleScene(BattleRoyaleClientMatchModel gameRoomData, string playerId)
        {
            log.Info("Матч найден");
            
            //Начать писать статистику боя
            NetworkStatisticsStorage.Instance.StartRecordingNewMatch(gameRoomData.MatchId, gameRoomData.PlayerTemporaryId);

            gameRoomData.GameServerIp = NetworkGlobals.GameServerIp;
            log.Info(nameof(gameRoomData.GameServerIp)+" "+gameRoomData.GameServerIp);
            log.Info(nameof(gameRoomData.GameServerPort)+" "+gameRoomData.GameServerPort);
            SetMatchData(gameRoomData);
            
            lobbySceneSwitcher.LoadSceneAsync("New3dBattleScene");
        }
        
        private static void SetMatchData(BattleRoyaleClientMatchModel matchData)
        {
            MyMatchDataStorage.Instance.SetMatchData(matchData);

            ushort playerTemporaryIdForOneMatch = matchData.PlayerTemporaryId;
            PlayerIdStorage.SetTmpPlayerId(playerTemporaryIdForOneMatch);
        }
    }
}
