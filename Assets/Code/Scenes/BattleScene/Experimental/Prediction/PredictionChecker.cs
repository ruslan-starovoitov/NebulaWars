using System;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// Проверяет совпадание положения игрока на снимке с сервера и на локально предсказанном снимке.
    /// </summary>
    public class PredictionChecker
    {
        private readonly PlayerEntityComparer playerEntityComparer;
        private readonly PredictedSnapshotsStorage predictedSnapshotsStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictionChecker));

        public PredictionChecker(PlayerEntityComparer playerEntityComparer,
            PredictedSnapshotsStorage predictedSnapshotsStorage)
        {
            this.playerEntityComparer = playerEntityComparer;
            this.predictedSnapshotsStorage = predictedSnapshotsStorage;
        }
        
        public bool IsPredictionCorrect(SnapshotWithLastInputId correctServerSnapshot, ushort playerAvatarId)
        {
            PredictedSnapshot predictedSnapshot = predictedSnapshotsStorage
                .GetByInputId(correctServerSnapshot.lastProcessedInputId);
            if (predictedSnapshot == null)
            {
                string mes = $"Не найдено предсказанное состояние. lastProcessedInputId = {correctServerSnapshot.lastProcessedInputId}";
                throw new Exception(mes);
            }
            
            bool timeIsSame = correctServerSnapshot.lastProcessedInputId == predictedSnapshot.lastInputId;
            if (!timeIsSame)
            {
                string message = "Время снимков не совпадает. " +
                                 $" inputId тика с сервера = {correctServerSnapshot.lastProcessedInputId} " +
                                 $" inputId предсказанного тика = {predictedSnapshot.lastInputId}";
                throw new ArgumentException(message);
            }
            
            bool isPredictionCorrect = playerEntityComparer
                .IsSame(predictedSnapshot, correctServerSnapshot, playerAvatarId);

            return isPredictionCorrect;
        }
    }
}