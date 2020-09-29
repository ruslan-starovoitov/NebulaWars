using Entitas;

namespace Plugins.submodules.SharedCode.LagCompensation
{
    /// <summary>
    /// Система по GameState расставляет GameObject-ы в соответствии с снимком
    /// </summary>
    public abstract class ArrangeTransformSystem
    {
        public abstract void Execute(Snapshot snapshot);
    }
}