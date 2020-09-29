using System;

namespace Plugins.submodules.SharedCode.Systems.MapInitialization
{
    public class WarshipSoValidator
    {
        public void Validate(WarshipSO warshipSo)
        {
            if (warshipSo.prefab == null)
            {
                throw new Exception("3d модель не установлена");
            }

            if (warshipSo.maxHealth <= 0)
            {
                throw new Exception("Недопустимый показатель здоровья "+warshipSo.maxHealth);
            }

            if (warshipSo.angularVelocity <= 0)
            {
                throw new Exception("Недопустимый показатель угловой скорости "+warshipSo.angularVelocity);
            }
            
            if (warshipSo.maxVelocity <= 0)
            {
                throw new Exception("Недопустимый показатель линейной скорости "+warshipSo.maxVelocity);
            }
        }
    }
}