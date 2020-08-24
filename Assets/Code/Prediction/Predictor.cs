using System;
using System.Collections.Generic;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Prediction
{
    public class PhysicsRollbackManager
    {
        public void Rollback(GameState gameState, ServerGameContext gameContext)
        {
            throw new NotImplementedException();
        }
    }
    public class Predictor
    {
        private readonly PhysicsScene physicsScene;
        private readonly ServerGameContext gameContext;
        private readonly InputMessagesHistory inputMessagesHistory;
        private readonly PhysicsRollbackManager physicsRollbackManager;
        private readonly LocalPredictionMoveHelper localPredictionMoveHelper;

        public Predictor(InputMessagesHistory inputMessagesHistory, PhysicsRollbackManager physicsRollbackManager,
            PhysicsScene physicsScene, PhysicsForceManager physicsForceManager, ServerGameContext gameContext)
        {
            this.inputMessagesHistory = inputMessagesHistory;
            this.physicsRollbackManager = physicsRollbackManager;
            this.physicsScene = physicsScene;
            this.gameContext = gameContext;


            localPredictionMoveHelper = new LocalPredictionMoveHelper(physicsForceManager);
        }
        
        public GameState Predict(GameState gameState, ushort playerEntityId)
        {
            //откатить физическую сцену к состоянию
            physicsRollbackManager.Rollback(gameState, gameContext);
            
            //взять ввод игрока
            List<InputMessageModel> inputMessageModels = inputMessagesHistory.Get(gameState.tickNumber);
            if (inputMessageModels.Count == 0)
            {
                throw new Exception("В истории нет ввода игрока за тик = "+gameState.tickNumber);
            }
            
            //todo двигать корабль игрока
            // Rigidbody warshipRigidbody = gameContext.GetEntityWithId(playerEntityId);
            
            // localPredictionMoveHelper.Move(warshipRigidbody, inputMessageModels);
            
            //todo спавн пуль игрока
            //todo движение пуль игрока
            
            //симуляция физики
            //todo длину интервала можно указать точно
            physicsScene.Simulate(100);
            throw new NotImplementedException();
        }
    }

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