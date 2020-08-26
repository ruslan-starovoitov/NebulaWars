using Code.Scenes.BattleScene.ECS.NewSystems;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing
{
    public class PingAnswerMessageHandler : MessageHandler<PingAnswerMessage>
    {
        private readonly IPingPresenter pingPresenter;
        private readonly ILog log = LogManager.CreateLogger(typeof(PingAnswerMessageHandler));

        public PingAnswerMessageHandler(IPingPresenter pingPresenter)
        {
            this.pingPresenter = pingPresenter;
        }
        
        protected override void Handle(in PingAnswerMessage message, uint messageId, bool needResponse)
        {
            pingPresenter.SetPing(message.pingMessageId);
        }
    }
}