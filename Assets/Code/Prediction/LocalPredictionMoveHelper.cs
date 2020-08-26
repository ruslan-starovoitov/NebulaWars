using System.Collections.Generic;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Prediction
{
    public class LocalPredictionMoveHelper
    {
        private readonly PhysicsForceManager physicsForceManager;

        public LocalPredictionMoveHelper(PhysicsForceManager physicsForceManager)
        {
            this.physicsForceManager = physicsForceManager;
        }
        
        public void Move(Rigidbody warshipRigidbody, List<InputMessageModel> inputMessageModels)
        {
            const float maxSpeed = 10f;
            const float forceMagnitude = 10f;
            
            List<Vector3> forceList = new List<Vector3>();
            foreach (InputMessageModel inputMessageModel in inputMessageModels)
            {
                Vector3 force = new Vector3(inputMessageModel.X, 0, inputMessageModel.Y) * forceMagnitude;
                forceList.Add(force);
            }

            foreach (var vector3 in forceList)
            {
                physicsForceManager.AddForce(vector3, warshipRigidbody, maxSpeed);
            }
        }
    }
}