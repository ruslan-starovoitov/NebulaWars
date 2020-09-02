using System;
using Code.Common.Storages;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class PlayerPredictionSystem : IExecuteSystem
    {
        private readonly PlayerPredictor playerPredictor;
        private readonly ClientInputMessagesHistory clientInputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerPredictionSystem));

        public PlayerPredictionSystem(Contexts contexts, ClientInputMessagesHistory clientInputMessagesHistory,
            PlayerPredictor playerPredictor)
        {
            this.clientInputMessagesHistory = clientInputMessagesHistory;
            this.playerPredictor = playerPredictor;
        }
        
        public void Execute()
        {
            try
            {
                ushort playerEntityId = PlayerIdStorage.PlayerEntityId;
                float deltaTime = Time.deltaTime;
                InputMessageModel inputMessageModel = clientInputMessagesHistory.GetLast();
                //тик
                playerPredictor.Predict(playerEntityId, inputMessageModel, deltaTime);
            }
            catch (Exception e)
            {
                log.Error(e.FullMessage());
            }
        }
    }
}