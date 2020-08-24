using System;
using System.Collections.Generic;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using UnityEngine;

namespace Code.Prediction
{
    public class PhysicsRollbackManager
    {
        public void Rollback(GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
    public class Predictor
    {
        private readonly PhysicsScene physicsScene;
        private readonly InputMessagesHistory inputMessagesHistory;
        private readonly PhysicsRollbackManager physicsRollbackManager;

        public Predictor(InputMessagesHistory inputMessagesHistory, PhysicsRollbackManager physicsRollbackManager,
            PhysicsScene physicsScene)
        {
            this.inputMessagesHistory = inputMessagesHistory;
            this.physicsRollbackManager = physicsRollbackManager;
            this.physicsScene = physicsScene;
        }
        
        public GameState Predict(GameState gameState)
        {
            //откатить физическую сцену к состоянию
            physicsRollbackManager.Rollback(gameState);
            //взять ввод игрока
            List<InputMessageModel> inputMessageModels = inputMessagesHistory.Get(gameState.tickNumber);
            if (inputMessageModels.Count == 0)
            {
                throw new Exception("В истории нет ввода игрока за тик = "+gameState.tickNumber);
            }
            
            //todo двигать корабль игрока
            Rigidbody warshipRigidbody = null;
            StubGasket stubGasket = new StubGasket();
            stubGasket.Move(warshipRigidbody, inputMessageModels);
            
            //todo спавн пуль игрока
            //todo движение пуль игрока
            
            //симуляция физики
            //todo длину интервала можно указать точно
            physicsScene.Simulate(100);
        }
    }
}