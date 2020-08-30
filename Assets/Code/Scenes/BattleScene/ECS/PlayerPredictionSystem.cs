using System.Collections.Generic;
using Code.Common.Storages;
using Code.Prediction;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.EntitasCore.Prediction;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class PlayerPredictionSystem : IExecuteSystem
    {
        private readonly PlayerPredictor playerPredictor;
        private readonly ClientInputMessagesHistory clientInputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerPredictionSystem));

        public PlayerPredictionSystem(Contexts contexts, ClientInputMessagesHistory clientInputMessagesHistory,
            PhysicsRollbackManager physicsRollbackManager, PhysicsScene physicsScene)
        {
            this.clientInputMessagesHistory = clientInputMessagesHistory;
            playerPredictor = new PlayerPredictor(clientInputMessagesHistory, physicsRollbackManager,
                physicsScene,contexts.serverGame);
        }
        
        public void Execute()
        {
            ushort playerEntityId = PlayerIdStorage.PlayerEntityId;
            float deltaTime = Time.deltaTime;
            InputMessageModel inputMessageModel = clientInputMessagesHistory.GetLast();
            //тик
            playerPredictor.Predict(playerEntityId, inputMessageModel, deltaTime);
        }
    }
}