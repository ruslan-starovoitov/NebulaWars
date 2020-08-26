using System;
using System.Collections.Generic;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Prediction
{
    public class PlayerPredictor
    {
        private readonly PhysicsScene physicsScene;
        private readonly ServerGameContext gameContext;
        private readonly InputMessagesHistory inputMessagesHistory;
        private readonly PhysicsRollbackManager physicsRollbackManager;
        private readonly LocalPredictionMoveHelper localPredictionMoveHelper;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerPredictor));

        public PlayerPredictor(InputMessagesHistory inputMessagesHistory, PhysicsRollbackManager physicsRollbackManager,
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
            // //откатить физическую сцену к состоянию
            // physicsRollbackManager.Rollback(gameState, gameContext);
            //
            // var inputMessageModels = inputMessagesHistory.Get(gameState.tickNumber);
            // Predict(playerEntityId, inputMessageModels, 100);
            throw new NotImplementedException();
        }

        public void Predict(ushort playerEntityId, List<InputMessageModel> inputMessageModels, float deltaTimeSec)
        {
            //взять ввод игрока
            if (inputMessageModels.Count == 0)
            {
                log.Debug("Нет ввода");
                return;
            }
            
            //todo двигать корабль игрока
            ServerGameEntity playerEntity = gameContext.GetEntityWithId(playerEntityId);
            Rigidbody warshipRigidbody = playerEntity.rigidbody.value;
            localPredictionMoveHelper.Move(warshipRigidbody, inputMessageModels);
            
            //todo спавн пуль игрока
            //todo движение пуль игрока
            
            //симуляция физики
            physicsScene.Simulate(deltaTimeSec);
        }
    }
}