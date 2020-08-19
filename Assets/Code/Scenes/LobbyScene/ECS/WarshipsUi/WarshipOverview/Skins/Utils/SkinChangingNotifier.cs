using System.Net.Http;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipOverview.Skins.Utils
{
    public class SkinChangingNotifier
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(SkinChangingNotifier));
        public async Task ChangeSkinOnServerAsync(int warshipId, string skinName)
        {
            log.Info($"Отправка собщения о смене скина {nameof(warshipId)} {warshipId} {nameof(skinName)} {skinName}");
            HttpClient httpClient = new HttpClient();
            if(!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
            {
                log.Warn("Не удалось получить id игрока");
                return;
            }
            
            using (MultipartFormDataContent formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(playerServiceId), "playerServiceId");
                formData.Add(new StringContent(warshipId.ToString()), "warshipId");
                formData.Add(new StringContent(skinName), "skinName");
                await httpClient.PostAsync(NetworkGlobals.ChangeSkinUrl, formData);
            }
        }
    }
}