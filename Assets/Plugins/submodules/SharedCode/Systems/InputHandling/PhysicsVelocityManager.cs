using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    /// <summary>
    /// Правильно устанавливает скорость кораблю.
    /// </summary>
    public class PhysicsVelocityManager
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(PhysicsVelocityManager));
        
        public void ApplyVelocity(Rigidbody rigidbody, Vector3 averageInputVector, float maxSpeed)
        {
            Vector3 velocityVector = maxSpeed * averageInputVector;
            if (maxSpeed < velocityVector.magnitude)
            {
                velocityVector = velocityVector.normalized * maxSpeed;   
            }
            
            rigidbody.velocity = velocityVector;
        }
    }
}