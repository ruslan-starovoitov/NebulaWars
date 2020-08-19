using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class DeliveryConfirmationSender
    {
        private readonly UdpSendUtils udpSendUtils;

        public DeliveryConfirmationSender(UdpSendUtils udpSendUtils)
        {
            this.udpSendUtils = udpSendUtils;
        }
        
        public void Send(MessageWrapper messageWrapper)
        {
            udpSendUtils.SendDeliveryConfirmationMessage(messageWrapper.MessageId);
        }
    }
}