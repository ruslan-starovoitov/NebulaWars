using Plugins.submodules.EntitasCore.Prediction;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Prediction;

namespace Code.Prediction
{
    public class ServerGameStateData
    {
        public FullSnapshot SerializedGameState { get; set; }
    }
}