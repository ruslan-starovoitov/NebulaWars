using System;
using Plugins.submodules.SharedCode.Logger;

namespace Plugins.submodules.SharedCode.LagCompensation
{
    public class PredictedSnapshot:Snapshot
    {
        public readonly uint lastInputId;
        public readonly DateTime dateTime;
        public readonly float physicsSimulationDuration;
        
        public PredictedSnapshot(DateTime dateTime, uint lastInputId, float physicsSimulationDuration)
        {
            this.dateTime = dateTime;
            this.lastInputId = lastInputId;
            this.physicsSimulationDuration = physicsSimulationDuration;
        }
    }
}