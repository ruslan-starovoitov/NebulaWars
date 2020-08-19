using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using ZeroFormatter;

namespace Code.Scenes.LobbyScene.Scripts.AccountModel
{
    public class LobbyModelLoader
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(LobbyModelLoader));
        
        public async Task<LobbyModel> Load(CancellationToken cts)
        {
            log.Info("Старт скачивания модели аккаунта");
            using (HttpClient httpClient = new HttpClient())
            {
                int attemptNumber = 0;
                while (true)
                {
                    if (attemptNumber != 0)
                    {
                        await Task.Delay(3000, cts);
                    }
                    log.Info("Номер попытки "+attemptNumber++);
                    
                    try
                    {
                        if (!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
                        {
                            log.Error("Не удалось получить id игрока");
                            continue;
                        }
                        else
                        {
                            log.Error("id игрока получен");
                        }

                        if (!PlayerIdStorage.TryGetUsername(out string username))
                        {
                            log.Error("Не удалось получить username игрока");
                            continue;
                        }

                        HttpResponseMessage response;
                        using (MultipartFormDataContent formData = new MultipartFormDataContent())
                        {
                            formData.Add(new StringContent(playerServiceId), nameof(playerServiceId));
                            formData.Add(new StringContent(username), nameof(username));
                            response = await httpClient.PostAsync(NetworkGlobals.InitializeLobbyUrl, formData, cts);
                        }


                        if (!response.IsSuccessStatusCode)
                        {
                            log.Error("Статус ответа " + response.StatusCode);
                            continue;
                        }

                        string base64String = await response.Content.ReadAsStringAsync();
                        if (base64String.Length == 0)
                        {
                            log.Error("Ответ пуст ");
                            continue;
                        }

                        byte[] data = Convert.FromBase64String(base64String);
                        log.Info("Длина ответа в байтах " + data.Length);
                        LobbyModel result = ZeroFormatterSerializer.Deserialize<LobbyModel>(data);
                        log.Info("Десериализация прошла нормально");
                        return result;
                    }
                    catch (TaskCanceledException e)
                    {
                        log.Error("Task был отменён");
                    }
                    catch (Exception e)
                    {
                        UiSoundsManager.Instance().PlayError();
                        log.Error("Упало при скачивании модели "+e.Message +" "+e.StackTrace);
                    }
                }
            }
        }
    }
}