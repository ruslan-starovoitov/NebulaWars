using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems
{
    public class PlayerInputSenderSystem : IExecuteSystem
    {
        private readonly UdpSendUtils udpSendUtils;
        private readonly InputMessagesHistory inputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerInputSenderSystem));

        public PlayerInputSenderSystem(UdpSendUtils udpSendUtils, 
            InputMessagesHistory inputMessagesHistory)
        {
            this.udpSendUtils = udpSendUtils;
            this.inputMessagesHistory = inputMessagesHistory;
        }

        public void Execute()
        {
            InputMessagesPack pack = inputMessagesHistory.GetInputModelsPack();
            udpSendUtils.SendInputPack(pack);
        }
    }
}