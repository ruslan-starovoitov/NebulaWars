using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class TransformMessageHandler : ITransformStorage
    {
        private readonly ITimeUpdater timeUpdater;
        private readonly ISnapshotCatalog snapshotCatalog;

        public TransformMessageHandler(ISnapshotCatalog snapshotCatalog, ITimeUpdater timeUpdater)
        {
            this.snapshotCatalog = snapshotCatalog;
            this.timeUpdater = timeUpdater;
        }
        
        public void SetNewTransforms(TransformPackMessage message)
        {
            SnapshotWithLastInputId snapshot = new SnapshotWithLastInputId(message);
            snapshotCatalog.Add(snapshot);

            timeUpdater.NewSnapshotReceived(message.TickNumber, message.TickStartTimeSec);
        }
    }
}