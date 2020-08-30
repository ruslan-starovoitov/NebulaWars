using System;
using Code.Scenes.BattleScene.Inperpolation;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using Plugins.submodules.SharedCode.Prediction;
using UnityEngine;

namespace Plugins.submodules.EntitasCore.Prediction
{
    public static class Interpolator
    {
        /// <param name="t">Параметр прогресса между p1 и p2. Принимает значение в диапазоне от 0 до 1</param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static FullSnapshot Interpolate(float t, FullSnapshot p0, FullSnapshot p1, FullSnapshot p2, FullSnapshot p3)
        {
            if (t < 0 || 1 < t)
            {
                throw new ArgumentOutOfRangeException($"t was {t}");
            }
            
            FullSnapshot result = new FullSnapshot();
            foreach (ushort entityId in p2.transforms.Keys)
            {
                bool v0Exists = p0.transforms.TryGetValue(entityId, out var v0);
                bool v1Exists = p1.transforms.TryGetValue(entityId, out var v1);
                //always true
                bool v2Exists = p2.transforms.TryGetValue(entityId, out var v2);
                bool v3Exists = p3.transforms.TryGetValue(entityId, out var v3);

                ViewTransformCompressed interpolated;
                //Сколько точек есть для интерполяции?
                if (v0Exists && v1Exists && v2Exists && v3Exists)
                {
                    //интерполяция между черытьмя точками
                    Vector3 position = CatmullRomSpline.GetCatmullRomPosition(t, v0.GetPosition(), v1.GetPosition(), v2.GetPosition(), v3.GetPosition());
                    float angle = Mathf.LerpAngle(v1.Angle, v2.Angle, t);
                    interpolated = new ViewTransformCompressed(position.x, position.z, angle, v2.viewTypeEnum);
                }
                else if(v1Exists && v2Exists)
                {
                    //интерполяция между двумя точками
                    Vector3 position = v1.GetPosition() * (1 - t) + v2.GetPosition() * t;
                    float angle = Mathf.LerpAngle(v1.Angle, v2.Angle, t);
                    interpolated = new ViewTransformCompressed(position.x, position.z, angle, v2.viewTypeEnum);
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