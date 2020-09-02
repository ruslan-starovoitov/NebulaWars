using System;
using System.Collections.Concurrent;
using System.Globalization;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Раз в n миллисекунд отправляет сообщение на игровой сервер для того, чтобы обновить ip адрес.
    /// </summary>
    public class PingSystem:IExecuteSystem
    {
        private int index;
        private DateTime nextPingTime;
        private readonly Text pingText;
        private readonly IPingStatisticsStorage pingStatisticsStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PingSystem));
        
        public PingSystem(Text pingText, IPingStatisticsStorage pingStatisticsStorage)
        {
            this.pingText = pingText;
            this.pingStatisticsStorage = pingStatisticsStorage;
        }
        
        public void Execute()
        {
            pingStatisticsStorage.TrySendPing();
            ShowPingTime();
        }

        private void ShowPingTime()
        {
            double lastPingSec = pingStatisticsStorage.GetLastPingSec();
            int lastPingMs = (int) lastPingSec;
            pingText.text = $"ping: {lastPingMs} ms";
        }
    }
}