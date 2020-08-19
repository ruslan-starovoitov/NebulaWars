using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;
using ZeroFormatter;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public abstract class MessageHandler<TMessage> : IMessageHandler 
        where TMessage : ITypedMessage
    {
        public void Handle(MessageWrapper messageWrapper)
        {
            var message = ZeroFormatterSerializer.Deserialize<TMessage>(messageWrapper.SerializedMessage);
            Handle(in message, messageWrapper.MessageId, messageWrapper.NeedResponse);
        }

        protected abstract void Handle(in TMessage message, uint messageId, bool needResponse);
    }
}