using Code.Scenes.BattleScene.Scripts;
using Libraries.NetworkLibrary.Udp.ServerToPlayer;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class HealthPointsPackHandler:MessageHandler<HealthPointsMessagePack>
    {
        private readonly IHealthPointsStorage healthPointsStorage;

        public HealthPointsPackHandler(IHealthPointsStorage healthPointsStorage)
        {
            this.healthPointsStorage = healthPointsStorage;
        }
        
        protected override void Handle(in HealthPointsMessagePack message, uint messageId, bool needResponse)
        {
            healthPointsStorage.SetNewHealthPoints(message);
        }
    }
}