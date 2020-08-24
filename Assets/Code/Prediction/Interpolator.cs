using System;
using Code.Scenes.BattleScene.Inperpolation;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Prediction
{
    public static class Interpolator
    {
        /// <param name="t">Параметр прогресса между p1 и p2. Принимает значение в диапазоне от 0 до 1</param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static GameState Interpolate(float t, GameState p0, GameState p1, GameState p2, GameState p3)
        {
            if (t < 0 || 1 < t)
            {
                throw new ArgumentOutOfRangeException($"t was {t}");
            }
            
            GameState result = new GameState();
            foreach (ushort entityId in p2.transforms.Keys)
            {
                bool v0Exists = p0.transforms.TryGetValue(entityId, out ViewTransform v0);
                bool v1Exists = p1.transforms.TryGetValue(entityId, out ViewTransform v1);
                //always true
                bool v2Exists = p2.transforms.TryGetValue(entityId, out ViewTransform v2);
                bool v3Exists = p3.transforms.TryGetValue(entityId, out ViewTransform v3);

                ViewTransform interpolated;
                //Сколько точек есть для интерполяции?
                if (v0Exists && v1Exists && v2Exists && v3Exists)
                {
                    //интерполяция между черытьмя точками
                    Vector3 position = CatmullRomSpline.GetCatmullRomPosition(t, v0.GetPosition(), v1.GetPosition(), v2.GetPosition(), v3.GetPosition());
                    float angle = Mathf.LerpAngle(v1.angle, v2.angle, t);
                    interpolated = new ViewTransform(position, angle, v2.viewTypeEnum);
                }
                else if(v1Exists && v2Exists)
                {
                    //интерполяция между двумя точками
                    Vector3 position = v1.GetPosition() * (1 - t) + v2.GetPosition() * t;
                    float angle = Mathf.LerpAngle(v1.angle, v2.angle, t);
                    interpolated = new ViewTransform(position, angle, v2.viewTypeEnum);
                }
                else
                {
                    //сущность была создана на этом кадре. интерполяция невозможна
                    interpolated = v2;
                }

                result.transforms.Add(entityId, interpolated);
            }

            return result;
        }
    }
}