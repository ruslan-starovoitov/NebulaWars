using System;
using System.Net.Http;
using System.Threading.Tasks;
using Code.Common;
using Code.Scenes.LobbyScene.Scripts.UiStorages;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.Scripts
{
    public class PurchaseValidatorTaskFactory
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(PurchaseValidatorTaskFactory));
        public async Task<bool> Create(string sku, string token)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                HttpResponseMessage response;
                using (MultipartFormDataContent formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(sku), "sku");
                    formData.Add(new StringContent(token), "token");
                    response = await httpClient.PostAsync(NetworkGlobals.ValidatePurchaseUrl, formData);
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Статус ответа "+response.StatusCode);
                }

            }
            catch (Exception e)
            {
                UiSoundsManager.Instance().PlayError();
                log.Error("Упало при скачивании модели "+e.Message +" "+e.StackTrace);
                return false;
            }

            return true;
        }
    }
}