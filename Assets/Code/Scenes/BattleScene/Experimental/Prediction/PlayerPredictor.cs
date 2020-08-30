using System;
using System.Collections.Generic;
using Code.Prediction;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Prediction;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PlayerPredictor
    {
        private readonly PhysicsScene physicsScene;
        private readonly ServerGameContext gameContext;
        private readonly PhysicsRollbackManager physicsRollbackManager;
        private readonly ClientInputMessagesHistory inputMessagesHistory;
        private readonly LocalPredictionMoveHelper localPredictionMoveHelper;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerPredictor));

        public PlayerPredictor(ClientInputMessagesHistory inputMessagesHistory, PhysicsRollbackManager physicsRollbackManager,
            PhysicsScene physicsScene,  ServerGameContext gameContext)
        {
            this.inputMessagesHistory = inputMessagesHistory;
            this.physicsRollbackManager = physicsRollbackManager;
            this.physicsScene = physicsScene;
            this.gameContext = gameContext;
            localPredictionMoveHelper = new LocalPredictionMoveHelper();
        }
        
        public FullSnapshot Predict(FullSnapshot FullSnapshot, ushort playerEntityId)
        {
            // //откатить физическую сцену к состоянию
            // physicsRollbackManager.Rollback(gameState, gameContext);
            //
            // var inputMessageModels = inputMessagesHistory.Get(gameState.tickNumber);
            // Predict(playerEntityId, inputMessageModels, 100);
            throw new NotImplementedException();
        }

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
            localPredictionMoveHelper.Move(warshipRigidbody, inputMessageModel);
            
            //вращательное движение игрока
            if (!float.IsNaN(inputMessageModel.Angle))
            {
                float angularVelocity = 0.5f;
                Quaternion currentRotation = playerEntity.rigidbody.value.rotation;
                Quaternion desiredRotation = Quaternion.Euler(0,inputMessageModel.Angle,0);
                Quaternion actualRotQ = Quaternion.RotateTowards(currentRotation, desiredRotation, angularVelocity); 
                playerEntity.rigidbody.value.MoveRotation(actualRotQ);    
            }

            //todo спавн пуль игрока
            //todo движение пуль игрока
            
            //симуляция физики
            physicsScene.Simulate(deltaTimeSec);
        }
    }
}