using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly SortedDictionary<DateTime, PredictedSnapshot> history =
            new SortedDictionary<DateTime, PredictedSnapshot>();

        public void PutPredicted(PredictedSnapshot predictedSnapshot)
        {
            var time = predictedSnapshot.dateTime;
            if (!history.ContainsKey(time))
            {
                history.Add(time, predictedSnapshot);
            }
            else
            {
                log.Error($"Игровое состояние с таким временем уже есть. time = {time}");
            }
        }
        
        public PredictedSnapshot GetByInputId(uint lastProcessedInputId)
        {
            if (history.Values.Count == 0)
            {
                log.Error($"Нет предсказанных состояний lastProcessedInputId {lastProcessedInputId}");
                return null;
            }
            
            List<PredictedSnapshot> list = history.Values.ToList();
            for (int index = list.Count - 1; index >= 0; index--)
            {
                PredictedSnapshot snapshot = list[index];
                if (snapshot.lastInputId == lastProcessedInputId)
                {
                    return snapshot;
                }
                
                if (snapshot.lastInputId < lastProcessedInputId)
                {
                    return list[index + 1];
                }
            }
            
            throw new Exception("Не удалось найти состояние по вводу. " +
                                $"lastProcessedInputId = {lastProcessedInputId} " +
                                $"кол-во предсказанных состояний = {list.Count} " +
                                $"maxInputId = {list.LastOrDefault()?.lastInputId} " +
                                $"minInputId = {list.FirstOrDefault()?.lastInputId} ");
        }

        public void PutCorrect(PredictedSnapshot predictedSnapshot)
        {
            if (history.ContainsKey(predictedSnapshot.dateTime))
            {
                history[predictedSnapshot.dateTime] = predictedSnapshot;
            }
            else
            {
                log.Error("Нет состояния с таким временем");
            }
        }
    }
}