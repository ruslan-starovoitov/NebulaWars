using System;
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
        private ISnapshotManager snapshotManager;
        private ITransformStorage transformStorage;
        private IHealthPointsStorage healthPointsStorage;
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
            
            if (!snapshotManager.IsReady())
            {
                int bufferLength = snapshotManager.GetBufferLength();
                log.Debug($"Буффер не заполнен. bufferLength = {bufferLength}");
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
            
            PhysicsVelocityManager physicsVelocityManager = new PhysicsVelocityManager(); 

            PhysicsRollbackManager physicsRollbackManager = new PhysicsRollbackManager();
            PhysicsRotationManager physicsRotationManager = new PhysicsRotationManager();
            PlayerPredictor playerPredictor = new PlayerPredictor(clientInputMessagesHistory, physicsRollbackManager, physicsScene,
                 contexts.serverGame, physicsVelocityManager, physicsRotationManager) ;
            PlayerEntityComparer playerEntityComparer = new PlayerEntityComparer();
            PredictedGameStateStorage predictedGameStateStorage = new PredictedGameStateStorage();
            var predictionManager = new PredictionManager(playerPredictor, predictedGameStateStorage, 
                playerEntityComparer, pingStatisticsStorage);

            Joystick movementJoystick = battleUiController.GetMovementJoystick();
            Joystick attackJoystick = battleUiController.GetAttackJoystick();

            
            
            
            LastInputIdStorage lastInputIdStorage = new LastInputIdStorage();
            
            
            
            SnapshotCatalog snapshotCatalog = new SnapshotCatalog();
            var snapshotInterpolator = new SnapshotInterpolator(snapshotCatalog);

            var consoleStub = new ConsoleNetworkProblemWarningView();
            NetworkTimeManager timeManager = new NetworkTimeManager(pingStatisticsStorage, snapshotCatalog,
                consoleStub);
            INetworkTimeManager networkTimeManager = timeManager;
            ITimeUpdater timeUpdater = timeManager;
            
            snapshotManager  = new SnapshotManager(snapshotCatalog, snapshotInterpolator);
            
            
            transformStorage = new TransformMessageHandler(snapshotCatalog, timeUpdater);
            
            
            MatchTimeSystem matchTimeSystem = new MatchTimeSystem(networkTimeManager);
            IMatchTimeStorage matchTimeStorage = matchTimeSystem;
            
            var updateTransformSystem = new UpdateTransformSystem(contexts, snapshotManager, matchTimeStorage);
            
            systems = new Entitas.Systems()
                    .Add(matchTimeSystem)
                    .Add(new ServerGameStateDebugSystem(snapshotCatalog, prefabsStorage))
                    .Add(new PredictionСheckSystem(snapshotCatalog, predictionManager))
                    .Add(updateTransformSystem)
                    .Add(updatePlayersSystem)
                    .Add(new PrefabSpawnerSystem(contexts, prefabsStorage, physicsSpawner))

                    .Add(new InputSystem(movementJoystick, attackJoystick, clientInputMessagesHistory,
                        snapshotManager, matchTimeStorage, lastInputIdStorage))
                    
                    .Add(new PlayerPredictionSystem(contexts, clientInputMessagesHistory, playerPredictor))
                    
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
                    
                    
                    
                    .Add(new InputSenderSystem(udpSendUtils, clientInputMessagesHistory))
                    .Add(new RudpMessagesSenderSystem(udpSendUtils))
                    .Add(new GameContextClearSystem(contexts))
                    .Add(new PredictedSnapshotHistoryUpdater(contexts, predictedGameStateStorage, matchTimeStorage,
                        lastInputIdStorage))
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
            if (transformStorage == null)
            {
                throw new NullReferenceException();
            }
            
            return transformStorage;
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