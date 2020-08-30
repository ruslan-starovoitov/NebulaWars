using System;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    public class InputModelValidator
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(InputModelValidator));
        public void Validate(InputMessageModel model)
        {
            if (Math.Abs(model.TickTimeSec) < 0.001)
            {
                log.Error("Пустое время тика");
            }
        }
    }
}