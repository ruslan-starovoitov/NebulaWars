using System;
using System.Collections.Concurrent;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Раз в n миллисекунд отправляет сообщение на игровой сервер для того, чтобы обновить ip адрес.
    /// </summary>
    public class PingSystem:IExecuteSystem, IPingPresenter
    {
        private int index;
        private DateTime nextPingTime;
        private readonly Text pingText;
        private readonly UdpSendUtils udpSendUtils;
        private readonly ILog log = LogManager.CreateLogger(typeof(PingSystem));
        private readonly ConcurrentQueue<string> showPing = new ConcurrentQueue<string>(); 
        private readonly ConcurrentDictionary<int, DateTime> pingSendingTime = new ConcurrentDictionary<int, DateTime>();
        
        public PingSystem(UdpSendUtils udpSendUtils, Text pingText)
        {
            this.udpSendUtils = udpSendUtils;
            this.pingText = pingText;
        }
        
        public void Execute()
        {
            TrySendPing();
            TryShowPingTime();
        }

        private void TrySendPing()
        {
            DateTime now = DateTime.UtcNow;
            if (nextPingTime < now)
            {
                pingSendingTime.TryAdd(index, now);
                udpSendUtils.SendPingMessage(index);
                nextPingTime = now + TimeSpan.FromSeconds(0.5f);
                index++;
            }
        }

        private void TryShowPingTime()
        {
            while (showPing.TryDequeue(out string result))
            {
                pingText.text = result;
            }
        }

        public void SetPing(int pingMessageId)
        {
            if (pingSendingTime.TryRemove(pingMessageId, out var time))
            {
                TimeSpan ping = DateTime.UtcNow - time;
                int pingMs = (int) ping.TotalMilliseconds;
                string str = $"{pingMs} ms";
                showPing.Enqueue(str);
            }
        }
    }
}