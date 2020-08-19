using System;
using System.Net.Http;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using ZeroFormatter;

namespace Code.Scenes.LobbyScene.Scripts
{
    public class UsernameChangingService
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(UsernameChangingService));
        
        public async Task<UsernameValidationResultEnum> ChangesUsernameAsync(string username)
        {
            if (!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
            {
                log.Warn("Не удалось получить id игрока");
                return UsernameValidationResultEnum.OtherError;
            }

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            using (MultipartFormDataContent formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(playerServiceId), nameof(playerServiceId));
                formData.Add(new StringContent(username), nameof(username));
                response = await httpClient.PostAsync(NetworkGlobals.ChangeUsernameUrl, formData);
            }


            if (!response.IsSuccessStatusCode)
            {
                log.Warn("Статус ответа " + response.StatusCode);
            }

            UsernameValidationResultEnum result = UsernameValidationResultEnum.OtherError;
            try
            {
                string base64String = await response.Content.ReadAsStringAsync();
                byte[] data = Convert.FromBase64String(base64String);
                result = ZeroFormatterSerializer.Deserialize<UsernameValidationResult>(data).UsernameValidationResultEnum;
            }
            catch (Exception e)
            {
                log.Error(e.Message+" "+e.StackTrace);
            }
            
            return result;
        }
    }
}