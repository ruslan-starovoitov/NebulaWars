using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Plugins.submodules.SharedCode.LagCompensation
{
    /// <summary>
    /// Хранит снимок с временем его создания
    /// </summary>
    public class SnapshotWithTime:Snapshot
    {
        public readonly int tickNumber;
        public readonly float tickTime;
        
        public SnapshotWithTime(int tickNumber, float tickTime)
        {
            this.tickNumber = tickNumber;
            this.tickTime = tickTime;
        }
    }

    public class SnapshotWithLastInputId : SnapshotWithTime
    {
        public readonly uint lastProcessedInputId;

        public SnapshotWithLastInputId(TransformPackMessage transformPackMessage)
            :base(transformPackMessage.TickNumber, transformPackMessage.TickStartTimeSec)
        {
            lastProcessedInputId = transformPackMessage.LastProcessedInputId;
            transforms = transformPackMessage.transform;
        }
        
        public SnapshotWithLastInputId(int tickNumber, float tickTime, uint lastProcessedInputId)
            : base(tickNumber, tickTime)
        {
            this.lastProcessedInputId = lastProcessedInputId;
        }
    }
}