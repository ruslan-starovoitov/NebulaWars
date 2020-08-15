using System.Collections.Generic;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public class ViewTypePathStorage
    {
        private readonly Dictionary<ViewTypeId, string> dict = new Dictionary<ViewTypeId, string>()
        {
            {ViewTypeId.StarSparrow, "Prefabs/3dWarships/StarSparrow1"},
            {ViewTypeId.DefaultShoot, "Prefabs/3dWarships/BlueLaserSmallOBJ"}
        };
        
        public string GetPath(ViewTypeId viewTypeId)
        {
            return dict[viewTypeId];
        }
    }
}