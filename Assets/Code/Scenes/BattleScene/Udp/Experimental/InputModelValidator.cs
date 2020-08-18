using System;
using Code.Common.Logger;
using NetworkLibrary.NetworkLibrary.Udp.PlayerToServer.UserInputMessage;

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    public class InputModelValidator
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(InputModelValidator));
        public void Validate(InputMessageModel model)
        {
            if (model.TickNumber == 0)
            {
                log.Error("Пустой номер тика");
            }
        }
    }
}