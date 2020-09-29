using System;
using System.Collections.Generic;

namespace SharedSimulationCode.Systems.Spawn
{
    public class ProjectileViewTypePathStorage
    {
        private readonly Dictionary<ViewTypeEnum, string> viewTypePrefabPath = new Dictionary<ViewTypeEnum, string>()
        {
            {ViewTypeEnum.DefaultShoot, "Shared/3dWarships/BlueLaserSmallOBJ"}
        };
        
        public string GetPath(ViewTypeEnum viewType)
        {
            if (!viewTypePrefabPath.ContainsKey(viewType))
            {
                throw new Exception("Нет такого ключа");
            }
            return viewTypePrefabPath[viewType];
        }
    }
}