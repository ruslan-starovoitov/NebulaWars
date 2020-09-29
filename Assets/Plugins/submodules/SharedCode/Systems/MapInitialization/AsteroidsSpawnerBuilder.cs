using System.Collections.Generic;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.MapInitialization
{
    /// <summary>
    /// Добавляет на карту набор астероидов
    /// </summary>
    public class AsteroidsSpawnerBuilder
    {
        private readonly AsteroidSpawnerHelper asteroidSpawnerHelper;

        public AsteroidsSpawnerBuilder(AsteroidSpawnerHelper asteroidSpawnerHelper)
        {
            this.asteroidSpawnerHelper = asteroidSpawnerHelper;
        }

        public void SpawnAsteroids()
        {
            float radius = 50;
            var random = new System.Random();
            List<ViewTypeEnum> asteroids = new List<ViewTypeEnum>()
            {
                ViewTypeEnum.AsteroidLavaBlue,
                ViewTypeEnum.AsteroidLavaRed,
                ViewTypeEnum.MineBlue,
                ViewTypeEnum.MineRed
            };
            for (int angle = 0; angle < 360; angle += 30)
            {
                float x = Mathf.Sin(angle)*radius;
                float z = Mathf.Cos(angle)*radius;
                ViewTypeEnum viewTypeEnum = asteroids[random.Next(0, 4)];
                asteroidSpawnerHelper.CreateAsteroid(new Vector3(x,0,z), viewTypeEnum);
            }
        }
    }
}