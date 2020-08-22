using System;
using System.Threading;

namespace Code.Scenes.BattleScene.Udp.Connection
{
    /// <summary>
    /// Нужен для симуляции сетевой задержки "Сервер-Клиент"
    /// </summary>
    public class ClientJitterSimulation : IByteArrayHandler
    {
        private readonly Random random;
        private readonly int minJitterDelayMs;
        private readonly int maxJitterDelayMs;
        private readonly IByteArrayHandler realByteArrayHandler;

        public ClientJitterSimulation(IByteArrayHandler realByteArrayHandler, int minJitterDelayMs, int maxJitterDelayMs)
        {
            this.realByteArrayHandler = realByteArrayHandler;
            this.minJitterDelayMs = minJitterDelayMs;
            this.maxJitterDelayMs = maxJitterDelayMs;
            random = new Random();
        }

        public void HandleBytes(byte[] data)
        {
            int delayMs = random.Next(minJitterDelayMs, maxJitterDelayMs);
            new Timer(callback: o => realByteArrayHandler.HandleBytes(data), null, delayMs,
                Timeout.Infinite);
        }
    }
}