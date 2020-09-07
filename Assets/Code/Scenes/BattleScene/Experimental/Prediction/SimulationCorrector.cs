using System;
using System.Collections.Generic;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class SimulationCorrector
    {
        private readonly PlayerPredictor playerPredictor;
        private readonly AverageInputManager averageInputManager;
        private readonly PredictedSnapshotsStorage predictedSnapshotsStorage;
        private readonly ClientInputMessagesHistory clientInputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(SimulationCorrector));
        private readonly PredictedSnapshotUtil predictedSnapshotUtil = new PredictedSnapshotUtil();

        public SimulationCorrector(PlayerPredictor playerPredictor, AverageInputManager averageInputManager, 
            ClientInputMessagesHistory clientInputMessagesHistory, PredictedSnapshotsStorage predictedSnapshotsStorage)
        {
            this.playerPredictor = playerPredictor;
            this.averageInputManager = averageInputManager;
            this.clientInputMessagesHistory = clientInputMessagesHistory;
            this.predictedSnapshotsStorage = predictedSnapshotsStorage;
        }

        public void Resimulate(SnapshotWithLastInputId correctServerSnapshot, 
            ushort playerAvatarId)
        {
            log.Info($"Пересчёт тика inputId = {correctServerSnapshot.lastProcessedInputId}.");

            ReplaceWrongSnapshot(correctServerSnapshot);
            ResimulateSnapshots(correctServerSnapshot, playerAvatarId);
        }

        private void ResimulateSnapshots(SnapshotWithLastInputId correctServerSnapshot, ushort playerAvatarId)
        {
            //достать все вводы после заменённого снимка
            var allInputs = clientInputMessagesHistory
                .GetAllFromId(correctServerSnapshot.lastProcessedInputId);
            List<AverageInputMessageModel> averageInputs = averageInputManager.GetAverageInputs(allInputs);

            log.Debug($"Кол-во вводов {allInputs.Count}. Кол-во тиков физики = {averageInputs.Count}");
            DateTime startTime = DateTime.UtcNow;

            //вызвать перегенерцию положения игрока для каждого ввода
            foreach (var averageInput in averageInputs)
            {
                //todo взять суммарную длительность тиков
                float physicsSimulationDuration = predictedSnapshotUtil
                    .GetTotalDuration(averageInput.replacedInputsIds, predictedSnapshotsStorage);
                log.Debug("Длительность симуляции "+physicsSimulationDuration);
                var baseSnapshot = predictedSnapshotsStorage.GetByInputId(averageInput.inputId - 1)
                                   ?? throw new NullReferenceException();
                

                if (baseSnapshot.lastInputId != averageInput.inputId - 1)
                {
                    throw new Exception("Не совпадает lastInputId");
                }

                log.Debug($"Пересчёт снимка {averageInput.inputId}");
                Snapshot snapshot = playerPredictor.Predict(baseSnapshot, playerAvatarId,
                    averageInput.inputMessageModel, physicsSimulationDuration);

                foreach (uint inputId in averageInput.replacedInputsIds)
                {
                    var wrongPredictedSnapshot = predictedSnapshotsStorage.GetByInputId(inputId)
                                                 ?? throw new NullReferenceException();
                    wrongPredictedSnapshot.Clear();
                    wrongPredictedSnapshot.Modify(snapshot);    
                }
            }

            DateTime finishTime = DateTime.UtcNow;
            int reconcileTime = (finishTime - startTime).Milliseconds;
            log.Debug($"reconcileTime = {reconcileTime}");
        }

        private void ReplaceWrongSnapshot(SnapshotWithLastInputId correctServerSnapshot)
        {
            PredictedSnapshot wrongSnapshot = predictedSnapshotsStorage
                .GetByInputId(correctServerSnapshot.lastProcessedInputId);
            //изменение ошибочного снимка
            // ReSharper disable once PossibleNullReferenceException
            wrongSnapshot.Clear();
            wrongSnapshot.Modify(correctServerSnapshot);

            //todo это можно не делать
            predictedSnapshotsStorage.PutCorrect(wrongSnapshot);
        }
    }
}