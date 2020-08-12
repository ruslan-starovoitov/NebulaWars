using Code.BattleScene.ECS.Systems;
using Code.Common.Logger;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS.Systems;
using Code.Scenes.BattleScene.ECS.Systems.AudioSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.ECS.Systems.TearDownSystems;
using Code.Scenes.BattleScene.ECS.Systems.ViewSystems;
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
    public class MatchEcsController:MonoBehaviour
    {
        private Systems systems;
        private Contexts contexts;
        private bool abilityButtonIsPressed;
        private UdpController udpControllerSingleton;
        private BattleUiController battleUiController;
        private readonly ILog log = LogManager.CreateLogger(typeof(MatchEcsController));
        
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
            //Возможно, стоит это вынести отдельно
            contexts.input.isTryingToUseAbility = abilityButtonIsPressed;
#if UNITY_EDITOR_WIN
            contexts.input.isTryingToUseAbility |= Input.GetKey(KeyCode.Space);
#endif

            systems.Execute();
            systems.Cleanup();
        }

        
        private Systems CreateSystems(UdpSendUtils udpSendUtils)
        {
            float prevFrameTime = Time.time - Time.deltaTime;
            int aliveCount = MyMatchDataStorage.Instance.GetMatchModel().PlayerModels.Length;
            
            contexts = Contexts.sharedInstance;
            systems = new Systems()
                    // .Add(new TimeSpeedSystem(contexts, new FloatLinearInterpolator(prevFrameTime)))

                    .Add(new UpdateTransformSystem(contexts))
                    // .Add(new DelayedSpawnSystem(contexts))
                    // .Add(new ManyDelayedRecreationsSystem(contexts))
                    // .Add(new DelayedRecreationSystem(contexts))
                    .Add(new UpdateRadiusSystem(contexts, new FloatLinearInterpolator(prevFrameTime)))
                    // .Add(new UpdateParentsSystem(contexts))
                    // .Add(new DetachParentsSystem(contexts))
                    // .Add(new UpdateHidingSystem(contexts))

                    // .Add(new GlobalTransformSystem(contexts))
                    .Add(new UpdatePlayersSystem(contexts))

                    // .Add(new AddViewSystem(contexts, battleUiController.GetGameViews()))
                    // .Add(new RenderSpriteSystem(contexts))
                    // .Add(new RenderCircleSystem(contexts))
                    // .Add(new RenderLineSystem(contexts))
                    // .Add(new AddNicknameDistanceSystem(contexts))
                    // .Add(new AddTextSystem(contexts, battleUiController.GetNicknameFontMaterial()))
                    // .Add(new SetAnimatorSystem(contexts))
                    // .Add(new GameObjectParentsCheckerSystem(contexts))
                    // .Add(new GameObjectDetachParentsSystem(contexts, battleUiController.GetGameViews()))
                    // .Add(new RenderLocalTransformSystem(contexts))
                    // .Add(new AddSpeedSystem(contexts))
                    // .Add(new RenderSmoothedTransformSystem(contexts))
                    // .Add(new RotateTextSystem(contexts))
                    // .Add(new MoveTextSystem(contexts))

                    // .Add(new UpdateParticlesSystem(contexts))

                    // .Add(new UpdateDestroysSystem(contexts))
                    // .Add(new DelayedDestroySystem(contexts))

                    // .Add(new GameObjectHidingSystem(contexts))

                    .Add(new SpawnSoundSystem(contexts))
                    //LoopSoundSystem?
                    .Add(new DeathSoundSystem(contexts))

                    // .Add(new DeathAnimationSystem(contexts))
                    // .Add(new DestroyTimerSubtractionSystem(contexts))
                    .Add(new DestroySystem(contexts))

                    .Add(new CameraAndBackgroundMoveSystem(contexts, battleUiController.GetMainCamera(),battleUiController.GetBackgrounds(), battleUiController.GetLoadingImage()))
                    // .Add(new MaterialsMoveSystem(contexts, battleUiController.GetMaterials()))
                    // .Add(new ZoneMoveSystem(contexts, battleUiController.GetZone()))
                    // .Add(new UpdateDirectionToCenterSystem(contexts, battleUiController.GetArrowToCenter()))
                    .Add(new JoysticksInputSystem(contexts, battleUiController.GetMovementJoystick(), battleUiController.GetAttackJoystick()))
                    .Add(new PlayerInputSenderSystem(contexts, udpSendUtils))
                    .Add(new AbilityInputClearingSystem(contexts))
                    .Add(new RudpMessagesSenderSystem(udpSendUtils))
                    .Add(new HealthAndShieldPointsUpdaterSystem(battleUiController.GetHealthSlider(),
                        battleUiController.GetHealthText(), battleUiController.GetShieldSlider(),
                        battleUiController.GetShieldText(), new FloatLinearInterpolator(prevFrameTime),
                                battleUiController.GetVignette()))
                     .Add(new KillsIndicatorSystem(battleUiController.GetKillMessage(), battleUiController.GetKillIndicator(), battleUiController.GetKillsText(), battleUiController.GetAliveText(), aliveCount))
                    // .Add(new CooldownsUpdaterSystem(battleUiController.GetCannonCooldownsController(), new FloatLinearInterpolator(prevFrameTime)))
                    .Add(new AbilityUpdaterSystem(battleUiController.GetAbilityCooldownInfo(), new FloatLinearInterpolator(prevFrameTime)))
                    .Add(new GameContextClearSystem(contexts))
                ;
            return systems;
        }

        public void AbilityButton_OnPointerDown()
        {
            abilityButtonIsPressed = true;
        }

        public void AbilityButton_OnPointerUp()
        {
            abilityButtonIsPressed = false;
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
            log.Info("Вызов остановки систем.");
            Destroy(this);
        }
    }
}