
namespace Plugins.submodules.SharedCode.LagCompensation
{
    /// <summary>
    /// Откатывает все физические обьекты к определённому моменту времени
    /// </summary>
    public interface ITimeMachine
    {
        SnapshotWithTime TravelToTime(int tickNumber);
        void SetActualGameState(SnapshotWithTime gameState);
    }
}