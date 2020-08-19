using System;
using System.Collections;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using Code.Scenes.LobbyScene.Scripts.UiStorages;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scenes.LobbyScene.Scripts.WarshipsUi
{
    public class WarshipImprovementPurchase:MonoBehaviour
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipImprovementPurchase));
        private LobbyEcsController lobbyEcsController;

        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>()
                                 ?? throw new Exception($"Не удалось найти {nameof(LobbyEcsController)}");
        }

        public void SendRequest(int warshipId)
        {
            StartCoroutine(FulfillRequest(warshipId));
        }

        private IEnumerator FulfillRequest(int warshipId)
        {
            if (!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
            {
                log.Error($"{nameof(FulfillRequest)} {nameof(playerServiceId)} is null");
                yield break;
            }
            
            (string name, string value)[] fields = 
            {
                (nameof(playerServiceId), playerServiceId),
                (nameof(warshipId), warshipId.ToString())
            };
            
            while (true)
            {
                //Отправь запрос
                Task<byte[]> task;
                try
                {
                    task = HttpWrapper.Post(NetworkGlobals.WarshipImprovementPurchasingUrl, fields);
                }
                catch (Exception e)
                {
                    UiSoundsManager.Instance().PlayError();
                    log.Error(e.Message);
                    continue;
                }
                
                //Жди пока выполнится (успешно/неуспешно)
                yield return new WaitUntil(()=>task.IsCompleted);

                if (task.IsCompleted)
                {
                    Destroy(lobbyEcsController);
                    SceneManager.LoadScene("LobbyScene");
                    break;
                }
                else
                {
                    log.Info("Не удалось выполнить транзакицю");
                }
            }
        }
    }
}