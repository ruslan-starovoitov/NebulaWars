using Plugins.submodules.SharedCode;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class SnapshotManager:ISnapshotManager
    {
        private readonly ISnapshotCatalog snapshotCatalog;
        private readonly ISnapshotInterpolator snapshotInterpolator;

        public SnapshotManager(ISnapshotCatalog snapshotCatalog,
            ISnapshotInterpolator snapshotInterpolator)
        {
            this.snapshotCatalog = snapshotCatalog;
            this.snapshotInterpolator = snapshotInterpolator;
        }

        public bool IsReady()
        {
            return snapshotCatalog.GetLength() >= 4;
        }

        public Snapshot CreateInterpolatedSnapshot(float matchTime)
        {
            Snapshot snapshot = snapshotInterpolator.Interpolate(matchTime);
            return snapshot;
        }

        public int? GetCurrentTickNumber(float matchTime)
        {
            return snapshotCatalog.GetTickNumberByTime(matchTime);
        }

        public int GetBufferLength()
        {
            return snapshotCatalog.GetLength();
        }
    }
}