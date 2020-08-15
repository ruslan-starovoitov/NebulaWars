using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
{
    public class HealthBarStorage
    {
        private GameObject cache;
        public GameObject GetPrefab()
        {
            if (cache == null)
            {
                cache = Resources.Load<GameObject>("Prefabs/Canvas_HealthBar");
            }

            return cache;
        }
    }
}