using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public interface IMessageHandler
    {
        void Handle(MessageWrapper messageWrapper);
    }
}