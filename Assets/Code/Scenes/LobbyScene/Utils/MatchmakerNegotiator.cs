using System;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using Code.Scenes.LobbyScene.Scripts;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using ZeroFormatter;

namespace Code.Scenes.LobbyScene.Utils
{
    /// <summary>
    /// Отвечает за отправку сообщений для
    /// - загрузки данных про бой
    /// - удаления игрока из очереди 
    /// </summary>
    public class MatchmakerNegotiator
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly LobbyEcsController lobbyEcsController;
        private readonly ILog log = LogManager.CreateLogger(typeof(MatchmakerNegotiator));

        public MatchmakerNegotiator(LobbyEcsController lobbyEcsController)
        {
            this.lobbyEcsController = lobbyEcsController;
        }

        public async Task<BattleRoyaleClientMatchModel> GetMatchDataAsync(CancellationToken cancellationToken, string playerId,
            int currentWarshipId)
        {
            for (int attemptNumber = 1; attemptNumber < 50; attemptNumber++)
            {
                try
                {
                    await Task.Delay(stopwatch.GetDelayNoMoreThanASecond(), cancellationToken);
                }
                catch (Exception e)
                {
                    log.Info("Отмена загрузки боя во время ожидания. ");
                }
                
                if (cancellationToken.IsCancellationRequested)
                {
                    log.Info("Была запрошен отмена.");
                    return null;
                }

                log.Info("Новая попытка получить данные матча ");
                string url = NetworkGlobals.GetMatchDataUrl;
                (string name, string value)[] fields = 
                {
                    (nameof(playerId), playerId),
                    ("warshipId", currentWarshipId.ToString())
                };

                byte[] data;
                try
                {
                    data = await HttpWrapper.Post(url, fields);
                }
                catch (Exception e)
                {
                    log.Error("Брошено исключение при отправке post запроса. " + e.Message);
                    continue;
                }
                
                if (data == null)
                {
                    log.Error("Пришёл пустой ответ от сервера");
                    continue;
                }
        
                try
                {
                    MatchmakerResponse response = ZeroFormatterSerializer.Deserialize<MatchmakerResponse>(data);
                    lobbyEcsController.SetNewMatchSearchData(response.NumberOfPlayersInQueue, response.NumberOfPlayersInBattles);
                    if (response.PlayerHasJustBeenRegistered)
                    {
                        log.Info("Игрок был зарегистрирован в матчмейкере");
                    }
                    else if (response.PlayerInQueue)
                    {
                        log.Info("Игрок находится в очереди.");
                    }
                    else if (response.PlayerInBattle)
                    {
                        log.Info("Игрок находится в бою.");
                        return response.MatchModel;
                    }
                }
                catch (Exception e)
                {
                    log.Error("не удалось десериализовать сообщение от сервера: " + e.Message);
                }
            }
            
            return null;
        }
        
        public async void DeleteFromQueueAsync()
        {
            log.Info("Отправка сообщения о покидании очереди.");
            string url = NetworkGlobals.DeleteFromQueueUrl;
            if (!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
            {
                log.Error($"{nameof(DeleteFromQueueAsync)} {nameof(playerServiceId)} is null");
                return;
            }
            
            if (playerServiceId == null)
            {
                throw new Exception($"{nameof(playerServiceId)} is null");
            }
            
            (string name, string value)[] fields = 
            {
                ("playerId", playerServiceId)
            };
            try
            {
                await HttpWrapper.Post(url, fields);
            }
            catch (Exception e)
            {
                log.Warn("Брошено исключение при отправке сообщения о покидании очереди. " + e.Message);
            }
        }
    }
}