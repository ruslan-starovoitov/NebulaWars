using System;
using Code.Common.NetworkStatistics;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.Scripts;
using Code.Scenes.BattleScene.Udp.Connection;
using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;
using ZeroFormatter;

namespace Code.Scenes.BattleScene.Udp
{
    /// <summary>
    /// Перенапрвляет сообщения от UdpClient к сортировщику сообщений
    /// </summary>
    public class ByteArrayHandler:IByteArrayHandler
    {
        private readonly EventProbability packetLossEvent;
        private readonly MessageWrapperHandler messageWrapperHandler;
        private readonly ILog log = LogManager.CreateLogger(typeof(ByteArrayHandler));

        public ByteArrayHandler(MessageWrapperHandler messageWrapperHandler)
        {
            this.messageWrapperHandler = messageWrapperHandler;
            packetLossEvent = new EventProbability(30);
        }
        
        public void HandleBytes(byte[] datagram)
        {
            //Для отладки на компьютере специально пропускаю пакеты
// #if UNITY_EDITOR
//             if (packetLossEvent.IsEventHappened())
//             {
//                 return;
//             }
// #endif
            
            MessagesPack messagesContainer = ZeroFormatterSerializer.Deserialize<MessagesPack>(datagram);
            NetworkStatisticsStorage.Instance.RegisterDatagram(datagram.Length, messagesContainer.Id);
            foreach (byte[] data in messagesContainer.Messages)
            {
                MessageWrapper message = ZeroFormatterSerializer.Deserialize<MessageWrapper>(data);
                NetworkStatisticsStorage.Instance.RegisterMessage(data.Length, message.MessageType);
                messageWrapperHandler.Handle(message);
            }
        }
    }
}