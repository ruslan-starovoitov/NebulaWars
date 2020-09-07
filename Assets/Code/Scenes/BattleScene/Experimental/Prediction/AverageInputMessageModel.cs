using System.Collections.Generic;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class AverageInputMessageModel
    {
        public InputMessageModel inputMessageModel;
        public List<uint> replacedInputsIds;
        public uint inputId;
    }
}