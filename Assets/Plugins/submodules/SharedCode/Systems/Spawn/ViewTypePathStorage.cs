using System.Collections.Generic;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    public class ViewTypePathStorage
    {
        private readonly Dictionary<ViewTypeEnum, string> dict = new Dictionary<ViewTypeEnum, string>()
        {
            {ViewTypeEnum.StarSparrow1, "Shared/3dWarships/StarSparrowCollider"},
            {ViewTypeEnum.DefaultShoot, "Shared/3dWarships/BlueLaserSmallOBJ"},
            {ViewTypeEnum.AsteroidLavaBlue, "Shared/3dWarships/AsteroidLavaBlue"},
            {ViewTypeEnum.AsteroidLavaRed, "Shared/3dWarships/AsteroidLavaRed"},
            {ViewTypeEnum.MineBlue, "Shared/3dWarships/MineSample1"},
            {ViewTypeEnum.MineRed, "Shared/3dWarships/MineSample2"},
        };
        
        public string GetPath(ViewTypeEnum viewTypeEnum)
        {
            return dict[viewTypeEnum];
        }
    }
}