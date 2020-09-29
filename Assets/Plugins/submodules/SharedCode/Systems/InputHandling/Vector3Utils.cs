using System;
using System.Collections.Generic;
using System.Linq;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    public class InputMessageModelUtil
    {
        public InputMessageModel GetAverage(List<InputMessageModel> inputMessageModels)
        {
            InputMessageModel result = new InputMessageModel();
            foreach (var model in inputMessageModels)
            {
                result.X += model.X;
                result.Y += model.Y;
                result.UseAbility |= model.UseAbility;
            }

            result.X = result.X / inputMessageModels.Count;
            result.Y = result.Y / inputMessageModels.Count;
            result.TickNumber = inputMessageModels.First().TickNumber;
            result.TickTimeSec = inputMessageModels.First().TickTimeSec;
            //todo угол считается не точно
            result.Angle = inputMessageModels.Last().Angle;
            return result;
        }
    }
    public class Vector3Utils
    {
        public Vector3 GetVelocityVector(List<Vector3> vectors)
        {
            if (vectors.Count == 0)
            {
                throw new Exception("Пустой список векторов");
            }

            Vector3 tmp = Vector3.zero;
            foreach (var vector in vectors)
            {
                tmp += vector;
            }

            return tmp / vectors.Count;
        }
    }
}