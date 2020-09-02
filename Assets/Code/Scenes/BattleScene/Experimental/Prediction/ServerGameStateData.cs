using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Prediction;

namespace Code.Prediction
{
    public class ServerGameStateData
    {
        public SnapshotWithLastInputId SerializedState { get; set; }
    }
}