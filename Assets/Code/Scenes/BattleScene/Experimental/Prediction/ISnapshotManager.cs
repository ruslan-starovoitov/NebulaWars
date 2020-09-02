using Plugins.submodules.SharedCode;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public interface ISnapshotManager
    {
        bool IsReady();
        Snapshot CreateInterpolatedSnapshot(float matchTime);
        int? GetCurrentTickNumber(float matchTime);
        int GetBufferLength();
    }
}