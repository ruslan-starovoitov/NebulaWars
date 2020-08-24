using System.Collections.Generic;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class HealthBarHeightStorage
    {
        private readonly Dictionary<ViewTypeEnum, float> dict = new Dictionary<ViewTypeEnum, float>()
        {
            {ViewTypeEnum.StarSparrow, 10},
            {ViewTypeEnum.AsteroidLavaBlue, 10},
            {ViewTypeEnum.AsteroidLavaRed, 10},
            {ViewTypeEnum.MineBlue, 10},
            {ViewTypeEnum.MineRed, 10}
        };

        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarHeightStorage));
        public float GetHeight(ViewTypeEnum viewTypeEnum)
        {
            if (dict.ContainsKey(viewTypeEnum))
            {
                return dict[viewTypeEnum];
            }

            log.Error("Нет информации про этот объект");
            return 0;
        }
    }
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