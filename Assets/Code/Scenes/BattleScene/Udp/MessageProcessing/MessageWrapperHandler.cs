using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scenes.BattleScene.ECS;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.Scripts;
using Code.Scenes.BattleScene.Udp.Connection;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing
{
    /// <summary>
    /// Перенаправляет входящее сообщение на нужный обработчик. 
    /// </summary>
    public class MessageWrapperHandler
    {
        private readonly IMessageHandler[] handlers;
        private readonly HashSet<uint> receivedMessagesRudp;
        private readonly DeliveryConfirmationSender deliveryConfirmationSender;
        private readonly ILog log = LogManager.CreateLogger(typeof(MessageWrapperHandler));

        public MessageWrapperHandler(UdpSendUtils udpSendUtils, int matchId, 
            ITransformStorage transformStorage, IPlayersStorage playersStorage,
            IHealthPointsStorage healthPointsStorage, 
            IMaxHealthPointsMessagePackStorage maxHealthPointsMessagePackStorage)
        {
            receivedMessagesRudp = new HashSet<uint>();
            deliveryConfirmationSender = new DeliveryConfirmationSender(udpSendUtils);
            var lastEnum = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().Max();
            handlers = new IMessageHandler[(int)lastEnum + 1];
            handlers[(int)MessageType.PlayerInfo] = new PlayerInfoMessageHandler(playersStorage);
            handlers[(int)MessageType.Positions] = new PositionsMessageHandler(transformStorage);
            // handlers[(int)MessageType.Radiuses] = new RadiusesMessageHandler();
            // handlers[(int)MessageType.Parents] = new ParentsMessageHandler();
            // handlers[(int)MessageType.Detaches] = new DetachesMessageHandler();
            // handlers[(int)MessageType.Destroys] = new DestroysMessageHandler();
            // handlers[(int)MessageType.Hides] = new HidesMessageHandler();
            handlers[(int)MessageType.HealthPointsMessagePack] = new HealthPointsPackHandler(healthPointsStorage);
            handlers[(int)MessageType.DeliveryConfirmation] = new RudpConfirmationReceiver();
            handlers[(int)MessageType.MaxHealthPoints] = new MaxHealthPointsHandler();
            // handlers[(int)MessageType.ShieldPoints] = new ShieldPointsHandler();
            // handlers[(int)MessageType.MaxShieldPoints] = new MaxShieldPointsHandler();
            // handlers[(int)MessageType.Kill] = new KillsHandler();
            handlers[(int)MessageType.ShowPlayerAchievements] = new ShowPlayerAchievementsHandler(matchId);
            // handlers[(int)MessageType.CooldownsInfos] = new CooldownsInfosHandler();
            // handlers[(int)MessageType.Cooldowns] = new CooldownsHandler();
            // handlers[(int)MessageType.FrameRate] = new FrameRateHandler();
            handlers[(int)MessageType.MaxHealthPointsMessagePack] = new MaxHealthPointsMessagePackHandler(maxHealthPointsMessagePackStorage);
        }
        
        public void Handle(MessageWrapper messageWrapper)
        {
            if (messageWrapper.NeedResponse)
            {
                deliveryConfirmationSender.Send(messageWrapper);
                //Если мы уже обработали это сообщение, то мы его пропускаем.
                if (!receivedMessagesRudp.Add(messageWrapper.MessageId))
                {
                    return;
                }
            }

            try
            {
                handlers[(int) messageWrapper.MessageType].Handle(messageWrapper);
            }
            catch (Exception e)
            {
                log.Error(e.FullMessage());
                log.Error(messageWrapper.MessageType.ToString());
            }
        }
    }
}