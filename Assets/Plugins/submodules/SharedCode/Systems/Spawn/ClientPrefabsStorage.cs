using System.Collections.Generic;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    public class ClientPrefabsStorage:IPrefabStorage
    {
        private readonly Dictionary<ViewTypeEnum, GameObject> cache = new Dictionary<ViewTypeEnum, GameObject>();
        private readonly Dictionary<ViewTypeEnum, string> dict = new Dictionary<ViewTypeEnum, string>
        {
            {ViewTypeEnum.StarSparrow1, "Shared/3dWarships/StarSparrowCollider"},
            {ViewTypeEnum.DefaultShoot, "Shared/3dWarships/BlueLaserSmallOBJ"},
            {ViewTypeEnum.AsteroidLavaBlue, "Shared/3dWarships/AsteroidLavaBlue"},
            {ViewTypeEnum.AsteroidLavaRed, "Shared/3dWarships/AsteroidLavaRed"},
            {ViewTypeEnum.MineBlue, "Shared/3dWarships/MineSample1"},
            {ViewTypeEnum.MineRed, "Shared/3dWarships/MineSample2"},
        };
        
        public GameObject GetPrefab(ViewTypeEnum viewTypeEnum)
        {
            string path = dict[viewTypeEnum];
            if (!cache.ContainsKey(viewTypeEnum))
            {
                cache[viewTypeEnum] = Resources.Load<GameObject>(path);
            }

            return cache[viewTypeEnum];
        }
    }
}