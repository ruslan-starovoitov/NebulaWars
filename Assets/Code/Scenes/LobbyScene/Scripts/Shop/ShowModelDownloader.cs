using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using ZeroFormatter;

namespace Code.Scenes.LobbyScene.Scripts.Shop
{
    /// <summary>
    /// Отвечает за скачивание модели магазина
    /// </summary>
    public class ShowModelDownloader
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(ShowModelDownloader));
        
        public async Task<ShopModel> GetShopModelAsync(CancellationToken cts)
        {
            log.Info("Старт скачивания модели магазина");
            HttpClient httpClient = new HttpClient();
            int attemptNumber = 0;
            while (true)
            {
                log.Info("Номер попытки "+attemptNumber++);
                await Task.Delay(1000, cts);
                try
                {
                    if(!PlayerIdStorage.TryGetServiceId(out var playerServiceId))
                    {
                        log.Warn("Не удалось получить id игрока");
                        continue;
                    }
                
                    string url = $"{NetworkGlobals.GetShopModel}?playerId={playerServiceId}";
                    HttpResponseMessage response = await httpClient.GetAsync(url, cts);
                    if (!response.IsSuccessStatusCode)
                    {
                        log.Warn("Статус ответа "+response.StatusCode);
                        continue;
                    }

                    string base64String = await response.Content.ReadAsStringAsync();
                    if (base64String.Length==0)
                    {
                        log.Warn("Ответ пуст ");
                        continue;
                    }

                    byte[] data = Convert.FromBase64String(base64String);
                    log.Info("Длина ответа в байтах "+data.Length);
                    ShopModel result = ZeroFormatterSerializer.Deserialize<ShopModel>(data);
                    log.Info("Десериализация прошла нормально");
                    return result;
                }
                catch (Exception e)
                {
                    log.Error("Упало при скачивании разметки магазина "+e.Message +" "+e.StackTrace);
                }
            }
        }
    }
}