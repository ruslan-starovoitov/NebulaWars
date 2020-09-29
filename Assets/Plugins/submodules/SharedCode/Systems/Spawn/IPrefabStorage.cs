using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    public interface IPrefabStorage
    {
        GameObject GetPrefab(ViewTypeEnum viewTypeEnum);
    }
}