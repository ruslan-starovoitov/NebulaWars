using System.Collections.Generic;
using System.Linq;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class LocalPredictionMoveHelper
    {
        public void Move(Rigidbody warshipRigidbody, InputMessageModel inputMessageModel)
        {
            const float maxSpeed = 10f;
            Vector3 direction = new Vector3(inputMessageModel.X, 0, inputMessageModel.Y);
            Vector3 velocityVector3 = direction * maxSpeed;
            warshipRigidbody.velocity = velocityVector3;
        }
    }
}