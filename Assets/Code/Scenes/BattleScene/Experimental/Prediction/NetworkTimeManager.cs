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
        private readonly ISnapshotCatalog snapshotCatalog;
        private readonly IPingStatisticsStorage pingStatisticsStorage;
        private readonly INetworkProblemWarningView networkProblemWarningView;
        private readonly ILog log = LogManager.CreateLogger(typeof(NetworkTimeManager));

        public NetworkTimeManager(IPingStatisticsStorage pingStatisticsStorage,
            ISnapshotCatalog snapshotCatalog, INetworkProblemWarningView networkProblemWarningView)
        {
            this.pingStatisticsStorage = pingStatisticsStorage;
            this.snapshotCatalog = snapshotCatalog;
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
            float newestSnapshotTickTime = snapshotCatalog.GetPenultimateSnapshotTickTime();
            if (newestSnapshotTickTime < showMatchTime)
            {
                string message = $"1 Ещё нет тика с таким временем " +
                                 $"{nameof(newestSnapshotTickTime)} = {newestSnapshotTickTime} " +
                                 $"запрашиваемое время = {showMatchTime}";
                log.Debug(message);
                log.Debug("2 Сдвиг времени матча назад");
                float delay = showMatchTime - newestSnapshotTickTime;
                log.Debug($"3 Величина сдвига = {delay}");
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
                
                log.Debug($"4 Новое значение задержки для накопления снимков = {snapshotBufferFillingDelay}");
                log.Debug("5 Пересчёт времени");
                //пересчитать время
                showMatchTime = GetSnowMatchTime();
                log.Debug($"6 Сдвинутое значение = {showMatchTime}");
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
                log.Debug($"Установка времени старта матча {estimatedMatchStartTime.ToLongTimeString()}");
            }
        }
    }
}