using System.Collections;
using System.Threading;
using Code.Common.Logger;
using Code.Scenes.BattleScene.Udp.Experimental;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Раз в n миллисекунд отправляет сообщение на игровой сервер для того, чтобы обновить ip адрес.
    /// </summary>
    [RequireComponent(typeof(UdpController))]
    public class PingController:MonoBehaviour
    {
        private static volatile CancellationTokenSource cts;
        private readonly ILog log = LogManager.CreateLogger(typeof(PingController));
        
        private void Start()
        {
            var udpController = GetComponent<UdpController>();
            UdpSendUtils udpSendUtils = udpController.GetUdpSendUtils();
            if (udpSendUtils == null)
            {
                log.Error("Отправка ping не работает.");
                return;
            }
            cts = new CancellationTokenSource();
            StartCoroutine(ServerPinging(udpSendUtils));
        }

        private void OnDestroy()
        {
            StopPing();
        }
        
        private IEnumerator ServerPinging(UdpSendUtils udpSendUtils)
        {
            while (!cts.IsCancellationRequested)
            {
                udpSendUtils.SendPingMessage();
                yield return new WaitForSeconds(0.5f);
            }
        }

        private void StopPing()
        {
            cts?.Cancel();
        }
    }
}