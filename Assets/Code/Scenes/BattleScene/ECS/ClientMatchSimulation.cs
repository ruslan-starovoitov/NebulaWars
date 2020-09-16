using System;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.NewSystems;
using Code.Scenes.BattleScene.ECS.Systems;
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
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using Plugins.submodules.SharedCode.Systems;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using Plugins.submodules.SharedCode.Systems.Spawn;
using UnityEngine;
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
        private IKillMessageStorage killMessageStorage;

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

        private Entitas.Systems CreateSystems(UdpSendUtils udpSendUtils, BattleRoyaleClientMatchModel matchModel,
            PingStatisticsStorage statisticsStorage)
        {
            GameSceneFactory gameSceneFactory = new GameSceneFactory();
            var matchScene = gameSceneFactory.Create();
            PhysicsSpawner physicsSpawner = new PhysicsSpawner(matchScene);
            contexts = Contexts.sharedInstance;
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
            ClientPrefabsStorage clientPrefabsStorage = new ClientPrefabsStorage();
            PhysicsVelocityManager physicsVelocityManager = new PhysicsVelocityManager();
            ArrangeTransformSystem[] arrangeCollidersSystems = 
            {
                new WithHpArrangeTransformSystem(contexts)
            };
            PhysicsRollbackManager physicsRollbackManager = new PhysicsRollbackManager(arrangeCollidersSystems);
            PhysicsRotationManager physicsRotationManager = new PhysicsRotationManager();
            SnapshotFactory snapshotFactory = new SnapshotFactory(contexts.serverGame);
            PlayerPredictor playerPredictor = new PlayerPredictor(physicsRollbackManager, matchScene,
                 contexts.serverGame, physicsVelocityManager, physicsRotationManager, snapshotFactory) ;
            PlayerEntityComparer playerEntityComparer = new PlayerEntityComparer();
            PredictedSnapshotsStorage predictedSnapshotsStorage = new PredictedSnapshotsStorage();
            AverageInputManager averageInputManager = new AverageInputManager();
            PredictionChecker predictionChecker = new PredictionChecker(playerEntityComparer, predictedSnapshotsStorage);
            SimulationCorrector simulationCorrector = new SimulationCorrector(playerPredictor, averageInputManager,
                clientInputMessagesHistory, predictedSnapshotsStorage);
            var predictionManager = new PredictionManager(predictionChecker, simulationCorrector);
            Joystick movementJoystick = battleUiController.GetMovementJoystick();
            Joystick attackJoystick = battleUiController.GetAttackJoystick();
            LastInputIdStorage lastInputIdStorage = new LastInputIdStorage();
            SnapshotBuffer snapshotBuffer = new SnapshotBuffer();
            var snapshotInterpolator = new SnapshotInterpolator(snapshotBuffer);
            var consoleStub = new ConsoleNetworkProblemWarningView();
            NetworkTimeManager timeManager = new NetworkTimeManager(pingStatisticsStorage, snapshotBuffer,
                consoleStub);
            INetworkTimeManager networkTimeManager = timeManager;
            ITimeUpdater timeUpdater = timeManager;
            snapshotManager  = new SnapshotManager(snapshotBuffer, snapshotInterpolator);
            transformStorage = new TransformMessageHandler(snapshotBuffer, timeUpdater);
            MatchTimeSystem matchTimeSystem = new MatchTimeSystem(networkTimeManager);
            IMatchTimeStorage matchTimeStorage = matchTimeSystem;
            var updateTransformSystem = new UpdateTransformSystem(contexts, snapshotManager, matchTimeStorage);
            SpawnManager spawnManager = new SpawnManager(clientPrefabsStorage, physicsSpawner);

            var killsIndicatorSystem = new KillsIndicatorSystem(battleUiController.GetKillMessage(), battleUiController.GetKillIndicator(),
                battleUiController.GetKillsText(), battleUiController.GetAliveText(), matchModel.PlayerModels.Length,
                new PlayerNameHelper(matchModel));

            killMessageStorage = killsIndicatorSystem;
            
            systems = new Entitas.Systems()
                    .Add(matchTimeSystem)
                    .Add(new ServerGameStateDebugSystem(snapshotBuffer, clientPrefabsStorage))
                    .Add(new PredictionСheckSystem(snapshotBuffer, predictionManager))
                    .Add(updateTransformSystem)
                    .Add(updatePlayersSystem)
                    
                    .Add(new PrefabSpawnerSystem(contexts, spawnManager))
                    .Add(new InputSystem(movementJoystick, attackJoystick, clientInputMessagesHistory, snapshotManager,
                        matchTimeStorage, lastInputIdStorage))
                    
                    .Add(new PlayerStopSystem(contexts))
                    .Add(new PlayerPredictionSystem(contexts, clientInputMessagesHistory, playerPredictor))
                    
                    .Add(new CameraMoveSystem(contexts, battleUiController.GetMainCamera(), cameraShift))
                    .Add(new LoadingImageSwitcherSystem(contexts, battleUiController.GetLoadingImage()))
                    
                    .Add(killsIndicatorSystem)
                    
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
                    .Add(new PredictedSnapshotHistoryUpdater(contexts, predictedSnapshotsStorage, matchTimeStorage, lastInputIdStorage))
                ;
            return systems;
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
                log.Info($"Буффер не заполнен. bufferLength = {bufferLength}");
                return;
            }

            if (PlayerIdStorage.PlayerEntityId == 0)
            {
                log.Info("Пустой id аккаунта.");
                return;
            }
            
            systems.Execute();
            systems.Cleanup();
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

        public IKillMessageStorage GetKillMessageStorage()
        {
            return killMessageStorage;
        }
    }
}