using System.Collections.Generic;
using System.Linq;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PredictedSnapshotUtil
    {
        public float GetTotalDuration(List<uint> predictedSnapshotIds,
            PredictedSnapshotsStorage predictedSnapshotsStorage)
        {
            float duration = predictedSnapshotIds
                .Sum(id => predictedSnapshotsStorage.GetByInputId(id).physicsSimulationDuration);

            return duration;
        }
    }
}