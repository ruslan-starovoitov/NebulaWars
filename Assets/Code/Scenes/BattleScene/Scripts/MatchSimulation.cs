using System;
using System.Collections.Generic;
using Code.BattleScene.ECS.Systems;
using Code.Common.Logger;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.Systems.EnvironmentSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.ECS.Systems.TearDownSystems;
using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Entitas;
using Libraries.NetworkLibrary.Udp.ServerToPlayer;
using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Отвечает за управление ecs системами.
    /// </summary>
    [RequireComponent(typeof(BattleUiController))]
    public class MatchSimulation:MonoBehaviour, ITransformStorage, IPlayersStorage, IHealthPointsStorage,
        IMaxHealthPointsMessagePackStorage
    {
        private Systems systems;
        private Contexts contexts;
        private UdpController udpControllerSingleton;
        private BattleUiController battleUiController;
        
        private UpdatePlayersSystem updatePlayersSystem;
        private HealthUpdaterSystem healthUpdaterSystem;
        private UpdateTransformSystem updateTransformSystem;
        private MaxHealthUpdaterSystem maxHealthUpdaterSystem;
        
        private readonly ILog log = LogManager.CreateLogger(typeof(MatchSimulation));

        private void Awake()
        {
            udpControllerSingleton = GetComponent<UdpController>();
            battleUiController = GetComponent<BattleUiController>();
        }

        private void Start()
        {
            UdpSendUtils udpSendUtils = udpControllerSingleton.GetUdpSendUtils();
            systems = CreateSystems(udpSendUtils);
            systems.ActivateReactiveSystems();
            systems.Initialize();
        }
        
        private void Update()
        {
            systems.Execute();
            systems.Cleanup();
        }

        private Systems CreateSystems(UdpSendUtils udpSendUtils)
        {
            contexts = Contexts.sharedInstance;
            var matchModel = MatchModelStorage.Instance.GetMatchModel();
            int aliveCount = MatchModelStorage.Instance.GetMatchModel().PlayerModels.Length;
            updateTransformSystem = new UpdateTransformSystem(contexts);
            updatePlayersSystem = new UpdatePlayersSystem(contexts, matchModel);
            
            healthUpdaterSystem = new HealthUpdaterSystem(contexts);
            maxHealthUpdaterSystem = new MaxHealthUpdaterSystem(contexts);
            Vector3 cameraShift = new Vector3(0, 60, -30);
            systems = new Systems()
                    .Add(updateTransformSystem)
                    .Add(updatePlayersSystem)
                    .Add(new PrefabSpawnerSystem(contexts))
                    
                    
                    .Add(new GameObjectsTransformUpdaterSystem(contexts))
                    .Add(new CameraMoveSystem(contexts, battleUiController.GetMainCamera(), cameraShift))
                    .Add(new LoadingImageSwitcherSystem(contexts, battleUiController.GetLoadingImage()))
                    
                    
                    
                    .Add(healthUpdaterSystem)
                    .Add(maxHealthUpdaterSystem)
                    
                    .Add(new HealthBarSpawnSystem(contexts, new HealthBarStorage()))
                    .Add(new HealthBarSliderUpdaterSystem(contexts))
                    .Add(new HealthBarRotatingSystem(contexts, cameraShift))
                    .Add(new HealthTextUpdatingSystem(contexts))
                    
                    
                    
                    
                    
                    .Add(new JoysticksInputSystem(contexts, battleUiController.GetMovementJoystick(), battleUiController.GetAttackJoystick()))
                    .Add(new PlayerInputSenderSystem(contexts, udpSendUtils))
                    // .Add(new AbilityInputClearingSystem(contexts))
                    .Add(new RudpMessagesSenderSystem(udpSendUtils))
                    // .Add(new KillsIndicatorSystem(battleUiController.GetKillMessage(), battleUiController.GetKillIndicator(), battleUiController.GetKillsText(), battleUiController.GetAliveText(), aliveCount))
                    // .Add(new AbilityUpdaterSystem(battleUiController.GetAbilityCooldownInfo()))
                    .Add(new GameContextClearSystem(contexts))
                ;
            return systems;
        }
        
        private void StopSystems()
        {
            systems.DeactivateReactiveSystems();
            systems.TearDown();
            systems.ClearReactiveSystems();
        }

        private void OnDestroy()
        {
            StopSystems();
        }

        /// <summary>
        /// Нужно для того, чтобы камера не перемещалась к последней позиции корабля а замерла на 0 0.
        /// </summary>
        public void SelfDestruct()
        {
            log.Info("Уничтожение ecs контроллера.");
            Destroy(this);
        }

        public void SetNewTransforms(uint messageId, Dictionary<ushort, ViewTransform> entitiesInfo)
        {
            updateTransformSystem.SetNewTransforms(messageId, entitiesInfo);
        }

        public void SetNewPlayers(Dictionary<int, ushort> newPlayers)
        {
            updatePlayersSystem.SetNewPlayers(newPlayers);
        }

        public void SetNewHealthPoints(HealthPointsMessagePack message)
        {
            healthUpdaterSystem.SetNewHealthPoints(message);
        }

        public void SetNewMaxHealthPoints(MaxHealthPointsMessagePack message)
        {
            maxHealthUpdaterSystem.SetNewMaxHealthPoints(message);
        }
    }
}