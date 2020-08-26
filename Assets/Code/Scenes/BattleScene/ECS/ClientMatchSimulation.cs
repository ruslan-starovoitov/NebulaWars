using System;
using Code.BattleScene.ECS.Systems;
using Code.Prediction;
using Code.Scenes.BattleScene.ECS.NewSystems;
using Code.Scenes.BattleScene.ECS.Systems.EnvironmentSystems;
using Code.Scenes.BattleScene.ECS.Systems.InputSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.ECS.Systems.TearDownSystems;
using Code.Scenes.BattleScene.Scripts.Ui;
using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
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
        private GameStateBuffer gameStateBuffer;
        private IHealthPointsStorage healthPointsStorage;
        private readonly BattleUiController battleUiController;
        private IMaxHealthPointsMessagePackStorage maxHealthPointsMessagePackStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(ClientMatchSimulation));

        public ClientMatchSimulation(BattleUiController battleUiController, UdpSendUtils udpSendUtils,
            BattleRoyaleClientMatchModel matchModel)
        {
            this.battleUiController = battleUiController;
            if (matchModel == null)
            {
                log.Error("Симуляция матча не запущена ");
                return;
            }
            systems = CreateSystems(udpSendUtils, matchModel);
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
            
            if (!gameStateBuffer.IsReady(out int bufferLength))
            {
                log.Info($"gameStateBuffer не готов. {nameof(bufferLength)} = {bufferLength}");
                return;
            }

            systems.Execute();
            systems.Cleanup();
        }

        private Entitas.Systems CreateSystems(UdpSendUtils udpSendUtils, BattleRoyaleClientMatchModel matchModel)
        {
            
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
            Scene matchScene = SceneManager.LoadScene("EmptyScene", loadSceneParameters);
            var physicsScene = matchScene.GetPhysicsScene();
            
            PhysicsSpawner physicsSpawner = new PhysicsSpawner(matchScene);
            PhysicsRaycaster physicsRaycaster = new PhysicsRaycaster(matchScene);
            PhysicsDestroyer physicsDestroyer = new PhysicsDestroyer();
            
            
            gameStateBuffer = new GameStateBuffer();
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
            pingSystem = new PingSystem(udpSendUtils, pingText);
            InputMessagesHistory inputMessagesHistory = new InputMessagesHistory(playerTmpId, matchId);
            PrefabsStorage prefabsStorage = new PrefabsStorage();

            PhysicsRollbackManager physicsRollbackManager = new PhysicsRollbackManager();
            PhysicsForceManager physicsForceManager = new PhysicsForceManager();
            PlayerPredictor playerPredictor = new PlayerPredictor(inputMessagesHistory, physicsRollbackManager, physicsScene,
                physicsForceManager, contexts.serverGame) ;
            PlayerEntityComparer playerEntityComparer = new PlayerEntityComparer();
            var predictionManager = new PredictionManager(playerPredictor, gameStateBuffer, playerEntityComparer);

            Joystick movementJoystick = battleUiController.GetMovementJoystick();
            Joystick attackJoystick = battleUiController.GetAttackJoystick();
            
            
            systems = new Entitas.Systems()
                    
                    .Add(new PredictionСheckSystem(gameStateBuffer, predictionManager))
                    .Add(new UpdateTransformSystem(contexts, gameStateBuffer))
                    .Add(updatePlayersSystem)
                    .Add(new PrefabSpawnerSystem(contexts, prefabsStorage, physicsSpawner))

                    .Add(new InputSystem(movementJoystick, attackJoystick, inputMessagesHistory, gameStateBuffer))
                    .Add(new PlayerPredictionSystem(contexts, inputMessagesHistory, physicsRollbackManager, physicsScene, physicsForceManager ))
                    
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
                    
                    
                    
                    .Add(new PlayerInputSenderSystem(udpSendUtils, inputMessagesHistory))
                    .Add(new RudpMessagesSenderSystem(udpSendUtils))
                    .Add(new GameContextClearSystem(contexts))
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

        public ITransformStorage GetITransformStorage()
        {
            if (gameStateBuffer == null)
            {
                throw new NullReferenceException();
            }
            
            return gameStateBuffer;
        } 
        
        public IPlayersStorage GetIPlayersStorage()
        {
            if (playersStorage == null)
            {
                throw new NullReferenceException();
            }

            return playersStorage;
        }
        
        public IHealthPointsStorage GetIHealthPointsStorage()
        {
            if (healthPointsStorage == null)
            {
                throw new NullReferenceException();
            }
            
            return healthPointsStorage;
        }
        
        public IMaxHealthPointsMessagePackStorage GetIMaxHealthPointsMessagePackStorage()
        {
            if (maxHealthPointsMessagePackStorage == null)
            {
                throw new NullReferenceException();
            }
            
            return maxHealthPointsMessagePackStorage;
        }

        public IPingPresenter GetPingPresenter()
        {
            return pingSystem;
        }
    }
}