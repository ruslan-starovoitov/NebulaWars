using System;
using System.Diagnostics;
using Code.BattleScene.ECS.Systems;
using Code.Prediction;
using Code.Scenes.BattleScene.ECS.NewSystems;
using Code.Scenes.BattleScene.ECS.Systems.EnvironmentSystems;
using Code.Scenes.BattleScene.ECS.Systems.InputSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.ECS.Systems.TearDownSystems;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Code.Scenes.BattleScene.Scripts.Ui;
using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.EntitasCore.Prediction;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using Plugins.submodules.SharedCode.Prediction;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using Plugins.submodules.SharedCode.Systems.Spawn;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.ECS
{
    public class ClientMatchSimulation
    {
        private Contexts contexts;
        private PingSystem pingSystem;
        private int lastSavedTickNumber;
        private Entitas.Systems systems;
        private IPlayersStorage playersStorage;
        private IHealthPointsStorage healthPointsStorage;
        private ServerGameStateBuffer serverGameStateBuffer;
        private readonly BattleUiController battleUiController;
        private readonly PingStatisticsStorage pingStatisticsStorage;
        private IMaxHealthPointsMessagePackStorage maxHealthPointsMessagePackStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(ClientMatchSimulation));

        public ClientMatchSimulation(BattleUiController battleUiController, UdpSendUtils udpSendUtils,
            BattleRoyaleClientMatchModel matchModel, PingStatisticsStorage pingStatisticsStorage)
        {
            this.battleUiController = battleUiController;
            this.pingStatisticsStorage = pingStatisticsStorage;
            if (matchModel == null)
            {
                log.Error("Симуляция матча не запущена ");
                return;
            }
            systems = CreateSystems(udpSendUtils, matchModel, pingStatisticsStorage);
            systems.ActivateReactiveSystems();
            systems.Initialize();
        }

        
        public void Tick()
        {
            if (systems == null)
            {
                return;
            }

            pingSystem.Execute();
            
            if (!serverGameStateBuffer.IsReady(out int bufferLength))
            {
                log.Info($"gameStateBuffer не готов. {nameof(bufferLength)} = {bufferLength}");
                return;
            }

            systems.Execute();
            systems.Cleanup();
        }

        private Entitas.Systems CreateSystems(UdpSendUtils udpSendUtils, BattleRoyaleClientMatchModel matchModel,
            PingStatisticsStorage statisticsStorage)
        {
            
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
            Scene matchScene = SceneManager.LoadScene("EmptyScene", loadSceneParameters);
            var physicsScene = matchScene.GetPhysicsScene();
            
            PhysicsSpawner physicsSpawner = new PhysicsSpawner(matchScene);
            PhysicsRaycaster physicsRaycaster = new PhysicsRaycaster(matchScene);
            PhysicsDestroyer physicsDestroyer = new PhysicsDestroyer();
            
            
            serverGameStateBuffer = new ServerGameStateBuffer();
            contexts = Contexts.sharedInstance;
            int aliveCount = matchModel.PlayerModels.Length;
            
            
            var updatePlayersSystem = new UpdatePlayersSystem(contexts, matchModel);
            playersStorage = updatePlayersSystem;
            
            
            var healthUpdaterSystem = new HealthUpdaterSystem(contexts);
            healthPointsStorage = healthUpdaterSystem;  
            var maxHealthUpdaterSystem = new MaxHealthUpdaterSystem(contexts);
            maxHealthPointsMessagePackStorage = maxHealthUpdaterSystem; 
            Vector3 cameraShift = new Vector3(0, 60, -30);
            ushort playerTmpId = matchModel.PlayerTemporaryId;
            int matchId = matchModel.MatchId;
            Text pingText = battleUiController.GetPingText();
            pingSystem = new PingSystem(pingText, statisticsStorage);
            ClientInputMessagesHistory clientInputMessagesHistory = new ClientInputMessagesHistory(playerTmpId, matchId);
            PrefabsStorage prefabsStorage = new PrefabsStorage();

            PhysicsRollbackManager physicsRollbackManager = new PhysicsRollbackManager();
            
            PlayerPredictor playerPredictor = new PlayerPredictor(clientInputMessagesHistory, physicsRollbackManager, physicsScene,
                 contexts.serverGame) ;
            PlayerEntityComparer playerEntityComparer = new PlayerEntityComparer();
            PredictedGameStateStorage predictedGameStateStorage = new PredictedGameStateStorage();
            var predictionManager = new PredictionManager(playerPredictor, predictedGameStateStorage, 
                playerEntityComparer, pingStatisticsStorage);

            Joystick movementJoystick = battleUiController.GetMovementJoystick();
            Joystick attackJoystick = battleUiController.GetAttackJoystick();

            var updateTransformSystem = new UpdateTransformSystem(contexts, serverGameStateBuffer);
            IMatchTimeStorage matchTimeStorage = updateTransformSystem;
            systems = new Entitas.Systems()
                    .Add(new ServerGameStateDebugSystem(serverGameStateBuffer, prefabsStorage))
                    // .Add(new PredictionСheckSystem(serverGameStateBuffer, predictionManager))
                    .Add(updateTransformSystem)
                    .Add(updatePlayersSystem)
                    .Add(new PrefabSpawnerSystem(contexts, prefabsStorage, physicsSpawner))

                    .Add(new InputSystem(movementJoystick, attackJoystick, clientInputMessagesHistory, serverGameStateBuffer, matchTimeStorage))
                    .Add(new PlayerPredictionSystem(contexts, clientInputMessagesHistory, physicsRollbackManager, physicsScene))
                    
                    .Add(new CameraMoveSystem(contexts, battleUiController.GetMainCamera(), cameraShift))
                    .Add(new LoadingImageSwitcherSystem(contexts, battleUiController.GetLoadingImage()))
                    
                    
                    .Add(healthUpdaterSystem)
                    .Add(maxHealthUpdaterSystem)
                    
                    .Add(new HealthBarSpawnSystem(contexts, new HealthBarStorage()))
                    .Add(new HealthBarSliderUpdaterSystem(contexts))
                    .Add(new HealthBarPositionUpdaterSystem(contexts))
                    .Add(new HealthBarRotatingSystem(contexts, cameraShift))
                    .Add(new HealthTextUpdatingSystem(contexts))
                    
                    
                    .Add(new HealthBarDestroyHelperSystem(contexts))
                    .Add(new DestroyViewSystem(contexts))
                    
                    
                    
                    .Add(new PlayerInputSenderSystem(udpSendUtils, clientInputMessagesHistory))
                    .Add(new RudpMessagesSenderSystem(udpSendUtils))
                    .Add(new GameContextClearSystem(contexts))
                    .Add(new GameStateJournalist(contexts, predictedGameStateStorage, matchTimeStorage))
                ;
            return systems;
        }
        
        public void StopSystems()
        {
            if (systems == null)
            {
                return;
            }
            
            systems.DeactivateReactiveSystems();
            systems.TearDown();
            systems.ClearReactiveSystems();
        }

        public ITransformStorage GetTransformStorage()
        {
            if (serverGameStateBuffer == null)
            {
                throw new NullReferenceException();
            }
            
            return serverGameStateBuffer;
        } 
        
        public IPlayersStorage GetPlayersStorage()
        {
            if (playersStorage == null)
            {
                throw new NullReferenceException();
            }

            return playersStorage;
        }
        
        public IHealthPointsStorage GetHealthPointsStorage()
        {
            if (healthPointsStorage == null)
            {
                throw new NullReferenceException();
            }
            
            return healthPointsStorage;
        }
        
        public IMaxHealthPointsMessagePackStorage GetMaxHealthPointsMessagePackStorage()
        {
            if (maxHealthPointsMessagePackStorage == null)
            {
                throw new NullReferenceException();
            }
            
            return maxHealthPointsMessagePackStorage;
        }
    }
}