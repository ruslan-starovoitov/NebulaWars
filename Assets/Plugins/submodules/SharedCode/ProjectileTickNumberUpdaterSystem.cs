using Entitas;

namespace Plugins.submodules.SharedCode
{
    /// <summary>
    /// Сдвигает время в котором должны быть расположены противники для снаряда
    /// </summary>
    public class ProjectileTickNumberUpdaterSystem : IExecuteSystem
    {
        private readonly IGroup<ServerGameEntity> tickNumberGroup;

        public ProjectileTickNumberUpdaterSystem(Contexts contexts)
        {
            tickNumberGroup = contexts.serverGame.GetGroup(ServerGameMatcher.TickNumber);
        }

        public void Execute()
        {
            foreach (var entity in tickNumberGroup)
            {
                entity.tickNumber.value += 1;
            }
        }
    }
}