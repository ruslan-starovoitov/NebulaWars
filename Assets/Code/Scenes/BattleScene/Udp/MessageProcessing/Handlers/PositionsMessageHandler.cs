using Code.BattleScene.ECS.Systems;
using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class PositionsMessageHandler:MessageHandler<PositionsMessage>
    {
        private readonly ITransformStorage transformStorage;

        public PositionsMessageHandler(ITransformStorage transformStorage)
        {
            this.transformStorage = transformStorage;
        }
        
        protected override void Handle(in PositionsMessage message, uint messageId, bool needResponse)
        {
            transformStorage.SetNewTransforms(messageId, message.entitiesInfo);
        }
    }
}