using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using UnityEditor.Experimental.GraphView;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems
{
    public class InputSenderSystem : IExecuteSystem
    {
        private readonly UdpSendUtils udpSendUtils;
        private readonly ClientInputMessagesHistory clientInputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(InputSenderSystem));

        public InputSenderSystem(UdpSendUtils udpSendUtils, 
            ClientInputMessagesHistory clientInputMessagesHistory)
        {
            this.udpSendUtils = udpSendUtils;
            this.clientInputMessagesHistory = clientInputMessagesHistory;
        }

        public void Execute()
        {
            InputMessagesPack pack = clientInputMessagesHistory.GetInputModelsPack();
            if (pack.History.Count == 0)
            {
                log.Debug("Не происходит отправка ввода.");
                return;
            }
            
            udpSendUtils.SendInputPack(pack);
        }
    }
}