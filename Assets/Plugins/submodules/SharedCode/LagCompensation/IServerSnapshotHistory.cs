namespace Plugins.submodules.SharedCode.LagCompensation
{
    public interface IServerSnapshotHistory
    {
        int GetLastTickNumber();
        void Add(SnapshotWithTime snapshotWithTime);
        SnapshotWithTime GetActualGameState();
        SnapshotWithTime Get(int tickNumber);
        float GetLastTickTime();
    }
}