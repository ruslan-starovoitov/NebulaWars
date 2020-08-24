using Code.BattleScene.ECS.Systems;
using Code.Common.Storages;
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

namespace Code.Scenes.BattleScene.ECS
{
    public interface IGameStateExecuteSystem
    {
        void Execute(GameState gameState);
    }
    public class ClientMatchSimulation
    {
        private Contexts contexts;
        private PingSystem pingSystem;
        private int lastSavedTickNumber;
        private Entitas.Systems systems;
        private GameStateBuffer gameStateBuffer;
        private PredictionManager predictionManager;
        private UpdatePlayersSystem updatePlayersSystem;
        private HealthUpdaterSystem healthUpdaterSystem;
        private MaxHealthUpdaterSystem maxHealthUpdaterSystem;
        private readonly BattleUiController battleUiController;
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


            // int bufferLastSavedTickNumber = gameStateBuffer.GetLastSavedTickNumber();
            //
            // //Пришла новая информация
            // if (lastSavedTickNumber < bufferLastSavedTickNumber)
            // {
            //     //Обновить локальный счётчик
            //     lastSavedTickNumber = bufferLastSavedTickNumber;
            //     GameState serverGameState = gameStateBuffer.GetLastGameState();
            //     int currentTickNumber = gameStateBuffer.GetCurrentTickNumber().Value;
            //     GameState currentClientGameState = gameStateBuffer.GetCurrentTargetGameState(currentTickNumber);
            //     ushort playerEntityId = PlayerIdStorage.PlayerEntityId;
            //     GameState correctGameState = predictionManager.Reconcile(currentTickNumber, serverGameState,
            //         currentClientGameState, playerEntityId);
            // }

            
            
          
            
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
            updatePlayersSystem = new UpdatePlayersSystem(contexts, matchModel);
            
            healthUpdaterSystem = new HealthUpdaterSystem(contexts);
            maxHealthUpdaterSystem = new MaxHealthUpdaterSystem(contexts);
            Vector3 cameraShift = new Vector3(0, 60, -30);
            ushort playerTmpId = matchModel.PlayerTemporaryId;
            int matchId = matchModel.MatchId;
            pingSystem = new PingSystem(udpSendUtils);
            InputMessagesHistory inputMessagesHistory = new InputMessagesHistory(playerTmpId, matchId);
            PrefabsStorage prefabsStorage = new PrefabsStorage();

            PhysicsRollbackManager physicsRollback = new PhysicsRollbackManager();
            PhysicsForceManager physicsForceManager = new PhysicsForceManager();
            Predictor predictor = new Predictor(inputMessagesHistory, physicsRollback, physicsScene,
                physicsForceManager, contexts.game) ;
            GameStateCopier gameStateCopier = new GameStateCopier();
            GameStateComparer gameStateComparer = new GameStateComparer();
            predictionManager = new PredictionManager(predictor, gameStateCopier, gameStateBuffer, gameStateComparer);

            systems = new Entitas.Systems()
                    .Add(new UpdateTransformSystem(contexts, gameStateBuffer))
                    .Add(updatePlayersSystem)
                    .Add(new PrefabSpawnerSystem(contexts, prefabsStorage, physicsSpawner))
                    
                    
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
                    
                    
                    .Add(new JoysticksInputSystem(contexts, battleUiController.GetMovementJoystick(), battleUiController.GetAttackJoystick()))
                    .Add(new PlayerInputSenderSystem(contexts, udpSendUtils, inputMessagesHistory, gameStateBuffer))
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
            return gameStateBuffer;
        } 
        
        public IPlayersStorage GetIPlayersStorage()
        {
            return updatePlayersSystem;
        }
        
        public IHealthPointsStorage GetIHealthPointsStorage()
        {
            return healthUpdaterSystem;
        }
        
        public IMaxHealthPointsMessagePackStorage GetIMaxHealthPointsMessagePackStorage()
        {
            return maxHealthUpdaterSystem;
        }
    }
}