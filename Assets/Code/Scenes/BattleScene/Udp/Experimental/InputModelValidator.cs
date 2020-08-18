using System;
using NetworkLibrary.NetworkLibrary.Udp.PlayerToServer.UserInputMessage;

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    public class InputModelValidator
    {
        public void Validate(InputMessageModel model)
        {
            if (model.TickNumber == 0)
            {
                throw new Exception("Пустой номер тика");
            }
        }
    }
}