using Code.BattleScene.ECS.Systems;
using Code.Common.Logger;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.Systems;
using Code.Scenes.BattleScene.ECS.Systems.AudioSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.ECS.Systems.TearDownSystems;
using Code.Scenes.BattleScene.Experimental.Approximation;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Отвечает за управление ecs системами.
    /// </summary>
    [RequireComponent(typeof(BattleUiController))]
    public class MatchSimulation:MonoBehaviour
    {
        private Systems systems;
        private Contexts contexts;
        private UdpController udpControllerSingleton;
        private BattleUiController battleUiController;
        private AbilityButtonListenerSystem abilitySystem;
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
            float prevFrameTime = Time.time - Time.deltaTime;
            int aliveCount = MyMatchDataStorage.Instance.GetMatchModel().PlayerModels.Length;
            abilitySystem = new AbilityButtonListenerSystem(contexts);
            contexts = Contexts.sharedInstance;
            systems = new Systems()
                    .Add(new UpdateTransformSystem(contexts))
                    .Add(new UpdateRadiusSystem(contexts, new FloatLinearInterpolator(prevFrameTime)))
                    .Add(new UpdatePlayersSystem(contexts))
                    .Add(new SpawnSoundSystem(contexts))
                    .Add(new DeathSoundSystem(contexts))
                    .Add(new DestroySystem(contexts))
                    .Add(new CameraAndBackgroundMoveSystem(contexts, battleUiController.GetMainCamera(),battleUiController.GetBackgrounds(), battleUiController.GetLoadingImage()))
                    .Add(new JoysticksInputSystem(contexts, battleUiController.GetMovementJoystick(), battleUiController.GetAttackJoystick()))
                    // .Add(abilitySystem)
                    .Add(new PlayerInputSenderSystem(contexts, udpSendUtils))
                    .Add(new AbilityInputClearingSystem(contexts))
                    .Add(new RudpMessagesSenderSystem(udpSendUtils))
                    .Add(new HealthAndShieldPointsUpdaterSystem(battleUiController.GetHealthSlider(), battleUiController.GetHealthText(), battleUiController.GetShieldSlider(), battleUiController.GetShieldText(), new FloatLinearInterpolator(prevFrameTime), battleUiController.GetVignette()))
                    .Add(new KillsIndicatorSystem(battleUiController.GetKillMessage(), battleUiController.GetKillIndicator(), battleUiController.GetKillsText(), battleUiController.GetAliveText(), aliveCount))
                    .Add(new AbilityUpdaterSystem(battleUiController.GetAbilityCooldownInfo(), new FloatLinearInterpolator(prevFrameTime)))
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
    }
}