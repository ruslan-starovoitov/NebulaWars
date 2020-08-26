using System.Collections.Generic;
using Code.Common.Storages;
using Code.Prediction;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class PlayerPredictionSystem : IExecuteSystem
    {
        private readonly PlayerPredictor playerPredictor;
        private readonly InputMessagesHistory inputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerPredictionSystem));

        public PlayerPredictionSystem(Contexts contexts, InputMessagesHistory inputMessagesHistory,
            PhysicsRollbackManager physicsRollbackManager, PhysicsScene physicsScene,
            PhysicsForceManager physicsForceManager)
        {
            this.inputMessagesHistory = inputMessagesHistory;
            playerPredictor = new PlayerPredictor(inputMessagesHistory, physicsRollbackManager,
                physicsScene, physicsForceManager, contexts.serverGame);
        }
        
        public void Execute()
        {
            ushort playerEntityId = PlayerIdStorage.PlayerEntityId;
            float deltaTime = Time.deltaTime;
            List<InputMessageModel> inputMessageModels = new List<InputMessageModel>()
            {
                inputMessagesHistory.GetLast()
            };
            //тик
            playerPredictor.Predict(playerEntityId, inputMessageModels, deltaTime);
        }
    }
}