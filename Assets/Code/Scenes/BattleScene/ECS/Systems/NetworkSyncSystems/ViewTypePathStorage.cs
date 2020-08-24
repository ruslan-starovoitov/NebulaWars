using System.Collections.Generic;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public class ViewTypePathStorage
    {
        private readonly Dictionary<ViewTypeEnum, string> dict = new Dictionary<ViewTypeEnum, string>()
        {
            {ViewTypeEnum.StarSparrow, "Shared/3dWarships/StarSparrowCollider"},
            {ViewTypeEnum.DefaultShoot, "Shared/3dWarships/BlueLaserSmallOBJ"}
        };
        
        public string GetPath(ViewTypeEnum ViewTypeEnum)
        {
            return dict[ViewTypeEnum];
        }
    }
}