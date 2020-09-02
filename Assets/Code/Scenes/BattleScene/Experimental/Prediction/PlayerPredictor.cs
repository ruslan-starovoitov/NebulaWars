using System;
using System.Collections.Generic;
using Code.Prediction;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.EntitasCore.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Prediction;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PlayerPredictor
    {
        private readonly PhysicsScene physicsScene;
        private readonly ServerGameContext gameContext;
        private readonly PhysicsVelocityManager physicsVelocityManager;
        private readonly PhysicsRotationManager physicsRotationManager;
        private readonly PhysicsRollbackManager physicsRollbackManager;
        private readonly ClientInputMessagesHistory inputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerPredictor));

        public PlayerPredictor(ClientInputMessagesHistory inputMessagesHistory, 
            PhysicsRollbackManager physicsRollbackManager, PhysicsScene physicsScene,  ServerGameContext gameContext,
            PhysicsVelocityManager physicsVelocityManager, PhysicsRotationManager physicsRotationManager)
        {
            this.inputMessagesHistory = inputMessagesHistory;
            this.physicsRollbackManager = physicsRollbackManager;
            this.physicsScene = physicsScene;
            this.gameContext = gameContext;
            this.physicsVelocityManager = physicsVelocityManager;
            this.physicsRotationManager = physicsRotationManager;
        }
        
        // public ClientSnapshot Predict(ClientSnapshot clientSnapshot, ushort playerEntityId)
        // {
        //     // //откатить физическую сцену к состоянию
        //     // physicsRollbackManager.Rollback(gameState, gameContext);
        //     //
        //     // var inputMessageModels = inputMessagesHistory.Get(gameState.tickNumber);
        //     // Predict(playerEntityId, inputMessageModels, 100);
        //     throw new NotImplementedException();
        // }

        public void Predict(ushort playerEntityId, InputMessageModel inputMessageModel, float deltaTimeSec)
        {
            //взять ввод игрока
            if (inputMessageModel == null)
            {
                log.Debug("Нет ввода");
                return;
            }
            
            //линейное движение игрока
            ServerGameEntity playerEntity = gameContext.GetEntityWithId(playerEntityId);
            Rigidbody warshipRigidbody = playerEntity.rigidbody.value;
            UnityEngine.Vector3 inputVector = inputMessageModel.GetVector3();
            float maxSpeed = 10f;
            physicsVelocityManager.ApplyVelocity(warshipRigidbody, inputVector, maxSpeed);
            
            //вращательное движение игрока
            if (!float.IsNaN(inputMessageModel.Angle))
            {
                physicsRotationManager.ApplyRotation(playerEntity.rigidbody.value, inputMessageModel.Angle, 3);
            }

            //todo спавн пуль игрока
            //todo движение пуль игрока
            
            //симуляция физики
            physicsScene.Simulate(deltaTimeSec);
        }
    }
}