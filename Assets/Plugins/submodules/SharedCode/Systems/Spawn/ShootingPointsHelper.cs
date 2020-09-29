using System.Collections.Generic;
using SharedSimulationCode;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    public class ShootingPointsHelper
    {
        public void AddShootingPoints(GameObject go, ServerGameEntity gameEntity)
        {
            List<Transform> shootingPoints = GetShootingPoints(go.transform);
            gameEntity.AddShootingPoints(shootingPoints);
        }
        
        private List<Transform> GetShootingPoints(Transform parent)
        {
            string tag = "ShootPosition";
            List<Transform> result = parent.GetChildrenWithTag(tag);
            return result;
        }
    }
}