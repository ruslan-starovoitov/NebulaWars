using Plugins.submodules.SharedCode;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class SnapshotManager:ISnapshotManager
    {
        private readonly ISnapshotBuffer snapshotBuffer;
        private readonly ISnapshotInterpolator snapshotInterpolator;

        public SnapshotManager(ISnapshotBuffer snapshotBuffer,
            ISnapshotInterpolator snapshotInterpolator)
        {
            this.snapshotBuffer = snapshotBuffer;
            this.snapshotInterpolator = snapshotInterpolator;
        }

        public bool IsReady()
        {
            return snapshotBuffer.GetLength() >= 4;
        }

        public Snapshot CreateInterpolatedSnapshot(float matchTime)
        {
            Snapshot snapshot = snapshotInterpolator.Interpolate(matchTime);
            return snapshot;
        }

        public int? GetCurrentTickNumber(float matchTime)
        {
            return snapshotBuffer.GetTickNumberByTime(matchTime);
        }

        public int GetBufferLength()
        {
            return snapshotBuffer.GetLength();
        }
    }
}