namespace Plugins.submodules.SharedCode.LagCompensation
{
    public abstract class LagCompensationSystem
    {
        // public abstract GameState PastState { get; set; }
        // public abstract GameState PresentState { get; set; }

        public abstract void Execute(ServerGameEntity entity);
    }
}