using System.Collections.Generic;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    public interface IConcreteSpawner:ISpawner
    {
        List<ViewTypeEnum> GetWhatCanSpawn();
    }
}