using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    public class PhysicsRotationManager
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(PhysicsRotationManager)); 
        
        public void ApplyRotation(Rigidbody rigidbody, float desiredAngle, float angularVelocity, float tickDeltaTime)
        {
            float value = tickDeltaTime * angularVelocity;
            Quaternion currentRotation = rigidbody.rotation;
            Quaternion desiredRotation = Quaternion.Euler(0,desiredAngle,0);
            Quaternion actualRotQ = Quaternion.RotateTowards(currentRotation, desiredRotation, value); 
            rigidbody.MoveRotation(actualRotQ);
        }
    }
}