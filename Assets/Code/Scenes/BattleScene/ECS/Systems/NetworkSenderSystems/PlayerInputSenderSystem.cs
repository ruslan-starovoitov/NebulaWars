using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSenderSystems
{
    public class PlayerInputSenderSystem : IExecuteSystem
    {
        private readonly UdpSendUtils udpSendUtils;
        private readonly IGroup<InputEntity> inputGroup;
        private readonly ITickNumberStorage tickNumberStorage;
        private readonly InputMessagesHistory inputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerInputSenderSystem));

        public PlayerInputSenderSystem(Contexts contexts, UdpSendUtils udpSendUtils, 
            InputMessagesHistory inputMessagesHistory, ITickNumberStorage tickNumberStorage)
        {
            this.udpSendUtils = udpSendUtils;
            this.inputMessagesHistory = inputMessagesHistory;
            this.tickNumberStorage = tickNumberStorage;
            inputGroup = contexts.input.GetGroup(InputMatcher
                .AnyOf(InputMatcher.Movement, InputMatcher.Attack, InputMatcher.TryingToUseAbility));
        }

        public void Execute()
        {
            int? tickNumber = tickNumberStorage.GetCurrentTickNumber();
            if (tickNumber == null)
            {
                log.Error("Слишком рано для вызова системы.");
                return;
            }
            
            
            float x = 0f, y = 0f, angle = float.NaN;
            bool useAbility = false;

            foreach (var inputEntity in inputGroup)
            {
                if (inputEntity.hasMovement)
                {
                    x = inputEntity.movement.x;
                    y = inputEntity.movement.y;
                }

                if (inputEntity.hasAttack)
                {
                    angle = inputEntity.attack.angle;
                }

                useAbility |= inputEntity.isTryingToUseAbility;
            }
            
            InputMessageModel inputMessageModel = new InputMessageModel()
            {
                Angle = angle,
                X = x,
                Y = y,
                UseAbility = useAbility,
                TickTimeMs = tickNumber.Value
            };
            
            inputMessagesHistory.AddInput(inputMessageModel);

            var pack = inputMessagesHistory.GetInputModelsPack();
            udpSendUtils.SendInputPack(pack);
        }
    }
}