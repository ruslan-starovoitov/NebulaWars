
namespace Plugins.submodules.SharedCode.LagCompensation
{
    /// <summary>
    /// Расставляет GameObject-ы по позициям согласно GameState
    /// </summary>
    public class TimeMachine : ITimeMachine
    {
        /// <summary>
        /// Текущее игровое состояние на сервере
        /// </summary>
        private SnapshotWithTime actualState;
        /// <summary>
        /// История игровых состояний
        /// </summary>
        private readonly IServerSnapshotHistory history;
        /// <summary>
        /// Набор систем, расставляющих коллайдеры в физическом мире по данным из игрового состояния
        /// </summary>
        private readonly ArrangeTransformSystem[] arrangeTransformSystems;

        public TimeMachine(IServerSnapshotHistory history, ArrangeTransformSystem[] arrangeTransformSystems)
        {
            this.history = history; 
            this.arrangeTransformSystems = arrangeTransformSystems;  
        }

        public void SetActualGameState(SnapshotWithTime gameState)
        {
            actualState = gameState;
        }

        public SnapshotWithTime TravelToTime(int tickNumber)
        {
            SnapshotWithTime pastState;
            if (tickNumber == actualState.tickNumber)
            {
                pastState = actualState;
            }
            else
            {
                pastState = history.Get(tickNumber);
            }
            
            foreach (var system in arrangeTransformSystems)
            {
                system.Execute(pastState);
            }
            
            return pastState;
        }
    }
}