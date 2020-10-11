using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// Проверяет, что автар игрока предсказан правильно. Если предсказан неправильно, то заменит неправильное
    /// состояние и пересчитает новые.
    /// </summary>
    public class PredictionManager
    {
        private readonly PredictionChecker predictionChecker;
        private readonly SimulationCorrector simulationCorrector;
        private readonly PredictedSnapshotsStorage predictedSnapshotsStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictionManager));

        public PredictionManager(PredictionChecker predictionChecker, SimulationCorrector simulationCorrector)
        {
            this.predictionChecker = predictionChecker;
            this.simulationCorrector = simulationCorrector;
        }

        public void Reconcile(SnapshotWithLastInputId correctServerSnapshot, ushort playerEntityId)
        {
            uint lastProcessedInputId = correctServerSnapshot.lastProcessedInputId;
            if (lastProcessedInputId == 0)
            {
                log.Debug("С сервера пришёл тик с пустым lastProcessedInputId");
                return;
            }

            if (!predictionChecker.IsPredictionCorrect(correctServerSnapshot, playerEntityId))
            {
                simulationCorrector.Resimulate(correctServerSnapshot, playerEntityId);
            }
            else
            {
                // log.Info($"Правильное предсказание inputId = {lastProcessedInputId}");
            }
        }
    }
}