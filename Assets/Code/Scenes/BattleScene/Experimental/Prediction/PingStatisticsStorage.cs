using System;
using System.Collections.Concurrent;
using Code.Scenes.BattleScene.Udp.Experimental;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PingStatisticsStorage:IPingStatisticsStorage
    {
        private DateTime nextPingTime;
        private int lastPingMessageId;
        private double lastPingValueSec;
        private readonly UdpSendUtils udpSendUtils;
        private readonly ConcurrentDictionary<int, DateTime> pingMessageSendingHistory 
            = new ConcurrentDictionary<int, DateTime>();
        
        public PingStatisticsStorage(UdpSendUtils udpSendUtils)
        {
            this.udpSendUtils = udpSendUtils;
        }
        
        public void TrySendPing()
        {
            DateTime now = DateTime.UtcNow;
            if (nextPingTime < now)
            {
                pingMessageSendingHistory.TryAdd(lastPingMessageId, now);
                udpSendUtils.SendPingMessage(lastPingMessageId);
                nextPingTime = now + TimeSpan.FromSeconds(0.5f);
                lastPingMessageId++;
            }
        }
        
        public float GetLastPingSec()
        {
            return (float)lastPingValueSec;
        }

        public void PingAnswer(int messagePingMessageId)
        {
            if (pingMessageSendingHistory.TryGetValue(messagePingMessageId, out DateTime sendingTime))
            {
                lastPingValueSec = (DateTime.UtcNow - sendingTime).TotalMilliseconds;
            }
        }
    }
}