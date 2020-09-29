using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode
{
    public class VectorValidator
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(VectorValidator));
        
        public bool TryValidate(Vector3 vector3)
        {
            if (!TryValidateFloat(vector3.x))
            {
                log.Error("x содержит мусор");
                return false;
            }
            
            if (!TryValidateFloat(vector3.y))
            {
                log.Error("y содержит мусор");
                return false;
            }
            
            if (!TryValidateFloat(vector3.z))
            {
                log.Error("z содержит мусор");
                return false;
            }

            return true;
        }

        private bool TryValidateFloat(float num)
        {
            if (float.IsNaN(num))
            {
                log.Error("num is NaN");
                return false;
            }

            if (float.IsInfinity(num))
            {
                log.Error("num is Infinity");
                return false;
            }
            
            return true;
        }
    }
}