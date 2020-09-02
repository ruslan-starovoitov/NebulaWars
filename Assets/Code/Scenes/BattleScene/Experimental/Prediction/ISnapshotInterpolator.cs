using Plugins.submodules.SharedCode;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public interface ISnapshotInterpolator
    {
        Snapshot Interpolate(float matchTime);
    }
}