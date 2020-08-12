using System;
using System.Collections.Generic;
using Code.BattleScene.ECS.Systems;
using Code.Common.Logger;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.Systems;
using Code.Scenes.BattleScene.ECS.Systems.AudioSystems;
using Code.Scenes.BattleScene.ECS.Systems.EnvironmentSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.ECS.Systems.TearDownSystems;
using Code.Scenes.BattleScene.Experimental.Approximation;
using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Entitas;
using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Отвечает за управление ecs системами.
    /// </summary>
    [RequireComponent(typeof(BattleUiController))]
    public class MatchSimulation:MonoBehaviour, ITransformStorage, IPlayersStorage
    {
        private Systems systems;
        private Contexts contexts;
        private UdpController udpControllerSingleton;
        private BattleUiController battleUiController;
        
        private UpdatePlayersSystem updatePlayersSystem;
        private AbilityButtonListenerSystem abilitySystem;
        private UpdateTransformSystem updateTransformSystem;
        
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
            Contexts.sharedInstance.game.ReplaceZoneInfo(Vector2.zero, 10f);
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
            abilitySystem = new AbilityButtonListenerSystem(contexts);
            updateTransformSystem = new UpdateTransformSystem(contexts);
            updatePlayersSystem = new UpdatePlayersSystem(contexts, matchModel);
            systems = new Systems()
                    .Add(updateTransformSystem)
                    .Add(updatePlayersSystem)
                    .Add(new PrefabSpawnerSystem(contexts))
                    
                    .Add(new SpawnSoundSystem(contexts))
                    .Add(new DeathSoundSystem(contexts))
                    .Add(new DestroySystem(contexts))
                    
                    .Add(new GameObjectsTransformUpdaterSystem(contexts))
                    .Add(new CameraMoveSystem(contexts, battleUiController.GetMainCamera()))
                    .Add(new LoadingImageSwitcherSystem(contexts, battleUiController.GetLoadingImage()))
                    .Add(new JoysticksInputSystem(contexts, battleUiController.GetMovementJoystick(), battleUiController.GetAttackJoystick()))
                    .Add(new PlayerInputSenderSystem(contexts, udpSendUtils))
                    .Add(new AbilityInputClearingSystem(contexts))
                    .Add(new RudpMessagesSenderSystem(udpSendUtils))
                    .Add(new KillsIndicatorSystem(battleUiController.GetKillMessage(), battleUiController.GetKillIndicator(), battleUiController.GetKillsText(), battleUiController.GetAliveText(), aliveCount))
                    .Add(new AbilityUpdaterSystem(battleUiController.GetAbilityCooldownInfo()))
                    .Add(new GameContextClearSystem(contexts))
                ;
            return systems;
        }

        public void AbilityButton_OnPointerDown()
        {
            abilitySystem.AbilityButton_OnClick();
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
    }
}