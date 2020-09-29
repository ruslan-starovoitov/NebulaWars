using System.Collections.Generic;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems
{
    public class RudpMessagesSenderSystem:IExecuteSystem
    {
        private readonly UdpSendUtils udpSendUtils;
        private readonly ILog log = LogManager.CreateLogger(typeof(RudpMessagesSenderSystem));

        public RudpMessagesSenderSystem(UdpSendUtils udpSendUtils)
        {
            this.udpSendUtils = udpSendUtils;
        }
        
        public void Execute()
        {
            ICollection<byte[]> messages = RudpStorage.Instance.GetReliableMessages();
            if (messages != null && messages.Count != 0)
            {
                foreach (var message in messages)
                {
                    log.Debug("Отправка rudp");
                    udpSendUtils.SendMessage(message);
                }
            }
        }
    }
}