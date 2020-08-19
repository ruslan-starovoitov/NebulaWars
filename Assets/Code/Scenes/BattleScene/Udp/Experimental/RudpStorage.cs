using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;
using ZeroFormatter;

//TODO я не смог в ioc поэтому впихнул синглтон. не делайте так.

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    public class RudpStorage
    {
        private static readonly Lazy<RudpStorage> Lazy = new Lazy<RudpStorage> (() => new RudpStorage());
        public static RudpStorage Instance => Lazy.Value;

        private RudpStorage()
        {
            unconfirmedMessages = new ConcurrentDictionary<uint, byte[]>();
        }
        //<messageId, Message>
        private readonly ConcurrentDictionary<uint, byte[]> unconfirmedMessages;

        public void AddMessage(MessageWrapper message)
        {
            byte[] serializedMessage = ZeroFormatterSerializer.Serialize(message);
            if (unconfirmedMessages.TryAdd(message.MessageId, serializedMessage))
            {
                //ignore   
            }
            else
            {
                throw new Exception("Не удалось добавить сообщение в rudp message.MessageId = "+message.MessageId);
            }
        }

        public ICollection<byte[]> GetReliableMessages()
        {
            return unconfirmedMessages.Values;
        }

        public void RemoveMessage(uint messageId)
        {
            if (unconfirmedMessages.TryRemove(messageId, out var message))
            {
                //ignore
            }
            else
            {
                throw new Exception("Не удалось удалить сообщение из rudp. messageId = "+messageId);
            }
        }
    }
}