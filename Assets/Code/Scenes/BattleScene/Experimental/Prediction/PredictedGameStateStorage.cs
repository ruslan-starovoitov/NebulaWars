using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// Хранит игровые состояния в которых позиция клиента предсказана.
    /// </summary>
    public class PredictedGameStateStorage
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictedGameStateStorage));
        //key is inputId 
        private readonly SortedDictionary<uint, PredictedSnapshot> history =
            new SortedDictionary<uint, PredictedSnapshot>();

        public void PutPredicted(PredictedSnapshot predictedSnapshot)
        {
            if (!history.ContainsKey(predictedSnapshot.lastInputId))
            {
                history.Add(predictedSnapshot.lastInputId, predictedSnapshot);
            }
            else
            {
                log.Error($"Игровое состояние с таким lastInputId уже есть. time = {predictedSnapshot.lastInputId}");
            }
        }
        
        /// <summary>
        /// todo плохо, что снимки могут изменяться вне хранилища
        /// </summary>
        /// <param name="lastProcessedInputId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [CanBeNull]
        public PredictedSnapshot GetByInputId(uint lastProcessedInputId)
        {
            if (history.Values.Count == 0)
            {
                log.Error($"Нет предсказанных состояний lastProcessedInputId {lastProcessedInputId}");
                return null;
            }

            if (history.TryGetValue(lastProcessedInputId, out var predictedSnapshot))
            {
                return predictedSnapshot;
            }
            else
            {
                return null;
            }
        }

        public void PutCorrect(PredictedSnapshot predictedSnapshot)
        {
            log.Debug("Замена плохо предсказанного тика");
            if (history.ContainsKey(predictedSnapshot.lastInputId))
            {
                history.Remove(predictedSnapshot.lastInputId);
                history[predictedSnapshot.lastInputId] = predictedSnapshot;
            }
            else
            {
                throw new Exception("В истории нет снимка с таким lastInputId.");
            }
        }
    }
}