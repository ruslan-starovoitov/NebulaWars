

using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class PositionsMessageHandler:MessageHandler<TransformPackMessage>
    {
        private readonly ITransformStorage transformStorage;

        public PositionsMessageHandler(ITransformStorage transformStorage)
        {
            this.transformStorage = transformStorage;
        }
        
        protected override void Handle(in TransformPackMessage message, uint messageId, bool needResponse)
        {
            transformStorage.SetNewTransforms(message);
        }
    }
}