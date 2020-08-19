using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.Scripts.WarshipsUi
{
    public class WarshipImprovementDownloader
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipImprovementDownloader));
        
        public async Task<bool> TryBuy(int warshipId)
        {
            HttpClient httpClient = new HttpClient();
            PlayerIdStorage.TryGetServiceId(out string playerServiceId);
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("warshipId", warshipId.ToString()), 
                new KeyValuePair<string, string>("playerServiceId", playerServiceId) 
            });
            string url = NetworkGlobals.WarshipImprovementPurchasingUrl;
            HttpResponseMessage result = await httpClient.PostAsync(url, formContent);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                log.Error($"Не удалось купить улучшение для корабля. " +
                          $"{nameof(result.StatusCode)} {result.StatusCode}");
                return false;
            }
        }
    }
}