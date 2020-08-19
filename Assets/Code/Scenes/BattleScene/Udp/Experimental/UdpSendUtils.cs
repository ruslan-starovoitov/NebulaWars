using System;
using System.Collections.Generic;
using Code.Common;
using Code.Common.Storages;
using Code.Scenes.BattleScene.Udp.Connection;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Common;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    /// <summary>
    /// Хранит последние n вводов игроков.
    /// Ввод отправляется несколько раз для того, чтобы сервер получил все вводы.
    /// </summary>
    public class InputMessagesHistory
    {
        private readonly int matchId;
        private readonly ushort playerTmpId;
        private readonly SelfCleaningDictionary<InputMessageModel> dict;
        private readonly InputModelValidator inputModelValidator = new InputModelValidator();
        private readonly InputMessageIdFactory inputMessageIdFactory = new InputMessageIdFactory();

        public InputMessagesHistory(ushort playerTmpId, int matchId)
        {
            this.playerTmpId = playerTmpId;
            this.matchId = matchId;
            dict = new SelfCleaningDictionary<InputMessageModel>(10);
        }
        
        public void AddInput(InputMessageModel model)
        {
            inputModelValidator.Validate(model);
            
            int inputId = inputMessageIdFactory.Create();
            dict.Add(inputId, model);
        }

        public InputMessagesPack GetInputModelsPack()
        {
            Dictionary<int, InputMessageModel> history = dict.Read();
            if (history.Count > 10)
            {
                throw new Exception("Коллекция работает неправильно");
            }
            
            InputMessagesPack pack = new InputMessagesPack()
            {
                MatchId = matchId,
                TemporaryId = playerTmpId,
                History = history
            };
            return pack;
        }
    }
    /// <summary>
    /// Принимает запросы на отправку сообщений от систем и перенаправляет их UdpClient-у
    /// </summary>
    public class UdpSendUtils
    {
        private readonly int matchId;
        private readonly UdpClientWrapper udpClientWrapper;
        
        public UdpSendUtils(int matchId, UdpClientWrapper udpClientWrapper)
        {
            this.matchId = matchId;
            this.udpClientWrapper = udpClientWrapper;
        }
        
        public void SendPingMessage()
        {
            var myId = PlayerIdStorage.TmpPlayerIdForMatch;
            var message = new PlayerPingMessage(myId, matchId);
            byte[] data = MessageFactory.GetSerializedMessage(MessageFactory.GetMessage(message,false, 
                out uint messageId));
            udpClientWrapper.Send(data);
        }
        
        public void SendDeliveryConfirmationMessage(uint messageNumberThatConfirms)
        {
            ushort myId = PlayerIdStorage.TmpPlayerIdForMatch;
            DeliveryConfirmationMessage message = new DeliveryConfirmationMessage
            {
                MessageNumberThatConfirms = messageNumberThatConfirms,
                PlayerId = myId,
                MatchId = matchId
            };
            MessageWrapper messageWrapper = MessageFactory.GetMessage(message, false, out uint messageId);
            byte[] data = MessageFactory.GetSerializedMessage(messageWrapper);
            udpClientWrapper.Send(data);
        }

        public void SendMessage(byte[] serializedMessage)
        {
            udpClientWrapper.Send(serializedMessage);
        }
        
        public void SendExitNotification()
        {
            var myId = PlayerIdStorage.TmpPlayerIdForMatch;
            BattleExitMessage exitMessage = new BattleExitMessage(matchId, myId);
            MessageWrapper message = MessageFactory.GetMessage(exitMessage, false, out uint messageId);
            byte[] data = MessageFactory.GetSerializedMessage(message);
            udpClientWrapper.Send(data);
        }

        public void SendInputPack(InputMessagesPack pack)
        {
            MessageWrapper message = MessageFactory.GetMessage(pack, false, out uint messageId);
            byte[] data = MessageFactory.GetSerializedMessage(message);
            udpClientWrapper.Send(data);
        }
    }
}