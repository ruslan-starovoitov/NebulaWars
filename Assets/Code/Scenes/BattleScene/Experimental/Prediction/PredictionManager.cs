using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// Проверяет, что автар игрока предсказан правильно. Если предсказан неправильно, то заменит неправильное
    /// состояние и пересчитает новые.
    /// </summary>
    public class PredictionManager
    {
        private readonly PlayerPredictor playerPredictor;
        private readonly PlayerEntityComparer playerEntityComparer;
        private readonly ClientInputMessagesHistory inputMessagesHistory;
        private readonly PredictedGameStateStorage predictedGameStateStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictionManager));

        public PredictionManager(PlayerPredictor playerPredictor, PredictedGameStateStorage predictedGameStateStorage,
            PlayerEntityComparer playerEntityComparer, ClientInputMessagesHistory inputMessagesHistory)
        {
            this.playerPredictor = playerPredictor;
            this.predictedGameStateStorage = predictedGameStateStorage;
            this.playerEntityComparer = playerEntityComparer;
            this.inputMessagesHistory = inputMessagesHistory;
        }

        public void Reconcile(SnapshotWithLastInputId correctServerSnapshot, ushort playerAvatarId)
        {
            uint lastProcessedInputId = correctServerSnapshot.lastProcessedInputId;
            if (lastProcessedInputId == 0)
            {
                log.Debug("С сервера пришёл тик с пустым lastProcessedInputId");
                return;
            }
            
            PredictedSnapshot predictedSnapshot = predictedGameStateStorage.GetByInputId(lastProcessedInputId);
            if (predictedSnapshot == null)
            {
                string mes = $"Не найдено предсказанное состояние. lastProcessedInputId = {lastProcessedInputId}";
                log.Error(mes);
                return;
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
            if (!isPredictionCorrect)
            {
                // log.Info($"Не правильное предсказание тика inputId = {lastProcessedInputId}." +
                //           $" Пересчёт позиции игрока.");

                //изменение ошибочного снимка
                predictedSnapshot.Clear();
                predictedSnapshot.Modify(correctServerSnapshot);

                //todo это можно не делать
                predictedGameStateStorage.PutCorrect(predictedSnapshot);

                //достать все вводы после заменённого снимка
                List<KeyValuePair<uint, InputMessageModel>> inputs = inputMessagesHistory
                    .GetAllFromId(lastProcessedInputId);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (var inputId in inputs.Select(pair=>pair.Key))
                {
                    stringBuilder.Append(" " + inputId);
                }
                // log.Info("Номера снимков для пересчёта "+stringBuilder);
                // log.Info("=============================================");
                // log.Info("=============================================");
                // log.Info("=============================================");
                //
                log.Debug("Кол-во снимков, которые будут пересчитаны " + inputs.Count);
                //todo тут можно сократить кол-во вызовов перегенерации физики за счёт суммирования векторов
                //и нахождения среднего
                
                
                // List<KeyValuePair<uint, InputMessageModel>> inputsForReconcile = new List<KeyValuePair<uint, InputMessageModel>>();
                //
                // //Из скольки векторов будет находится средний
                // //todo достать из истории тиков сервера
                // int clientFrequencyCoefficient = 6;
                // Vector3Utils vector3Utils = new Vector3Utils();
                // List<Vector3> velocityVectors = new List<Vector3>();
                // vector3Utils.GetVelocityVector()


                DateTime startTime = DateTime.UtcNow;
                //вызвать перегенерцию положения игрока для каждого ввода
                foreach (var pair in inputs)
                {
                    uint inputMessageId = pair.Key;
                    InputMessageModel inputMessageModel = pair.Value;

                    var correctPredictedSnapshot = predictedGameStateStorage.GetByInputId(inputMessageId - 1)
                                                ?? throw new NullReferenceException();
                    var wrongPredictedSnapshot = predictedGameStateStorage.GetByInputId(inputMessageId) 
                                                ?? throw new NullReferenceException();

                    if (correctPredictedSnapshot.lastInputId != inputMessageId - 1)
                    {
                        throw new Exception("Не совпадает lastInputId");
                    }
                    
                    if (wrongPredictedSnapshot.lastInputId != inputMessageId)
                    {
                        throw new Exception("Не совпадает lastInputId");
                    }
                    
                    log.Debug($"Пересчёт снимка {inputMessageId}");
                    Snapshot snapshot = playerPredictor.Predict(correctPredictedSnapshot, playerAvatarId,
                        inputMessageModel, wrongPredictedSnapshot.physicsSimulationDuration);


                    wrongPredictedSnapshot.Clear();
                    wrongPredictedSnapshot.Modify(snapshot);
                }
                
                // log.Debug("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                // log.Debug("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                // log.Debug("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                
                
                // log.Debug("Пересчёт снимков окончен.");
                
                DateTime finishTime = DateTime.UtcNow;

                int reconcileTime = (finishTime - startTime).Milliseconds;
                log.Debug($"reconcileTime = {reconcileTime}");
            }
            else
            {
                // log.Info($"Правильное предсказание inputId = {lastProcessedInputId}");
            }
        }
    }
}