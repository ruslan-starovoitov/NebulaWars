using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class SnapshotInterpolator:ISnapshotInterpolator
    {
        private readonly SnapshotCatalog snapshotCatalog;

        public SnapshotInterpolator(SnapshotCatalog snapshotCatalog)
        {
            this.snapshotCatalog = snapshotCatalog;
        }
        
        public Snapshot Interpolate(float matchTime)
        {
            snapshotCatalog.GetSnapshots(matchTime, 
                out SnapshotWithTime s0, out SnapshotWithTime s1,
                out SnapshotWithTime s2, out SnapshotWithTime s3);
            
            //[0, 1]
            float progress = (matchTime - s1.tickTime) / (s2.tickTime - s1.tickTime);
            Snapshot result = Interpolator.Interpolate(progress, s0, s1, s2, s3);
            return result;
        }
    }
}