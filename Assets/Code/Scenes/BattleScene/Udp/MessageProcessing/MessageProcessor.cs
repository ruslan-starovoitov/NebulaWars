using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using NetworkLibrary.NetworkLibrary.Udp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing
{
    /// <summary>
    /// Перенаправляет входящее сообщение на нужный обработчик. 
    /// </summary>
    public class MessageProcessor
    {
        private readonly IMessageHandler[] handlers;
        private readonly HashSet<uint> receivedMessagesRudp;
        private readonly RudpConfirmationSender rudpConfirmationSender;

        public MessageProcessor(UdpSendUtils udpSendUtils, int matchId)
        {
            receivedMessagesRudp = new HashSet<uint>();
            rudpConfirmationSender = new RudpConfirmationSender(udpSendUtils);
            var lastEnum = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().Max();
            handlers = new IMessageHandler[(int)lastEnum + 1];
            handlers[(int)MessageType.PlayerInfo] = new PlayerInfoMessageHandler();
            handlers[(int)MessageType.Positions] = new PositionsMessageHandler();
            handlers[(int)MessageType.Radiuses] = new RadiusesMessageHandler();
            handlers[(int)MessageType.Parents] = new ParentsMessageHandler();
            handlers[(int)MessageType.Detaches] = new DetachesMessageHandler();
            handlers[(int)MessageType.Destroys] = new DestroysMessageHandler();
            handlers[(int)MessageType.Hides] = new HidesMessageHandler();
            handlers[(int)MessageType.HealthPoints] = new HealthPointsHandler();
            handlers[(int)MessageType.DeliveryConfirmation] = new RudpConfirmationReceiver();
            handlers[(int)MessageType.MaxHealthPoints] = new MaxHealthPointsHandler();
            handlers[(int)MessageType.ShieldPoints] = new ShieldPointsHandler();
            handlers[(int)MessageType.MaxShieldPoints] = new MaxShieldPointsHandler();
            handlers[(int)MessageType.Kill] = new KillsHandler();
            handlers[(int)MessageType.ShowPlayerAchievements] = new ShowPlayerAchievementsHandler(matchId);
            handlers[(int)MessageType.CooldownsInfos] = new CooldownsInfosHandler();
            handlers[(int)MessageType.Cooldowns] = new CooldownsHandler();
            handlers[(int)MessageType.FrameRate] = new FrameRateHandler();
        }
        
        public void Handle(MessageWrapper messageWrapper)
        {
            if (messageWrapper.NeedResponse)
            {
                rudpConfirmationSender.Handle(messageWrapper);
                //Если мы уже обработали это сообщение, то мы его пропускаем.
                if (!receivedMessagesRudp.Add(messageWrapper.MessageId)) return;
            }
            
            
            handlers[(int)messageWrapper.MessageType].Handle(messageWrapper);
        }
    }
}