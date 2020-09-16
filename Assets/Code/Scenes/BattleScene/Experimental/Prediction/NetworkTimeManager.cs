using System;
using System.Text;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class NetworkTimeManager:INetworkTimeManager, ITimeUpdater
    {
        private DateTime? matchStartTime;
        private float interpolationDelaySec = 0.1f;
        private float snapshotBufferFillingDelay = 0;
        private readonly ISnapshotBuffer snapshotBuffer;
        private readonly IPingStatisticsStorage pingStatisticsStorage;
        private readonly INetworkProblemWarningView networkProblemWarningView;
        private readonly ILog log = LogManager.CreateLogger(typeof(NetworkTimeManager));

        public NetworkTimeManager(IPingStatisticsStorage pingStatisticsStorage,
            ISnapshotBuffer snapshotBuffer, INetworkProblemWarningView networkProblemWarningView)
        {
            this.pingStatisticsStorage = pingStatisticsStorage;
            this.snapshotBuffer = snapshotBuffer;
            this.networkProblemWarningView = networkProblemWarningView;
        }
        
        public bool IsReady()
        {
            return matchStartTime != null;
        }

        private float GetSnowMatchTime()
        {
            //посчитать время для показа
            DateTime now = DateTime.UtcNow;
            float clientMatchTime = (float) (now - matchStartTime.Value).TotalSeconds;
            float showMatchTime = clientMatchTime - interpolationDelaySec - snapshotBufferFillingDelay;
            return showMatchTime;
        }
        
        public float GetMatchTime()
        {
            float showMatchTime = GetSnowMatchTime();

            //проверить, что в буффере есть тик с таким временм
            float newestSnapshotTickTime = snapshotBuffer.GetPenultimateSnapshotTickTime();
            if (newestSnapshotTickTime < showMatchTime)
            {
                
                float delay = showMatchTime - newestSnapshotTickTime;
                string message = $" Ещё нет тика с таким временем " +
                                 $" {nameof(newestSnapshotTickTime)} = {newestSnapshotTickTime} " +
                                 $" запрашиваемое время = {showMatchTime}" +
                                 $" Сдвиг времени матча назад " +
                                 $" Величина сдвига = {delay}";
                //ещё нет тика с таким временем
                networkProblemWarningView.ShowWarning();
                //увеличить задержку для заполнения буффера
                snapshotBufferFillingDelay += delay+0.1f;
                if (snapshotBufferFillingDelay > 1)
                {
                    string mes = $"Задержка для заполнения буффера больше секунды. " +
                                 $"snapshotBufferFillingDelay = {snapshotBufferFillingDelay} " +
                                 $"Нужно выполнить полное переподключение. ";
                    log.Error(mes);
                }
                
                log.Debug($"Новое значение задержки для накопления снимков = {snapshotBufferFillingDelay}");
                //пересчитать время
                showMatchTime = GetSnowMatchTime();
                log.Debug($"Сдвинутое значение = {showMatchTime}");
                if (newestSnapshotTickTime < showMatchTime)
                {
                    string mes = $"7 Не удалось сдвинуть время назад." +
                                     $" самый новый тик = {newestSnapshotTickTime} " +
                                     $"запрашиваемое время  = {showMatchTime}";
                    throw new Exception(mes);
                }
            }
            
            return showMatchTime;
        }

        public void NewSnapshotReceived(int tickNumber, float tickMatchTime)
        {
            if (matchStartTime == null)
            {
                float lastPingSec = pingStatisticsStorage.GetLastPingSec();
                DateTime now = DateTime.UtcNow;
                // TimeSpan networkCorrection = TimeSpan.FromSeconds(lastPingSec/2);
                TimeSpan matchTimeCorrection = TimeSpan.FromSeconds(tickMatchTime);
                DateTime estimatedMatchStartTime = now 
                                                   // - networkCorrection
                                                   - matchTimeCorrection
                    ;
                matchStartTime = estimatedMatchStartTime;
                log.Info($"Установка времени старта матча {estimatedMatchStartTime.ToLongTimeString()}");
            }
        }
    }
}