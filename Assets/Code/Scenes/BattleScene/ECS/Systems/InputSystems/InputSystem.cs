using System;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.InputSystems
{
    public class InputSystem : IExecuteSystem
    {
        private readonly Joystick attackJoystick;
        private readonly Joystick movementJoystick;
        private readonly ITickNumberStorage tickNumberStorage;
        private readonly InputMessagesHistory inputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(InputSystem));

        public InputSystem(Joystick forMovement, Joystick forAttack,
            InputMessagesHistory inputMessagesHistory, ITickNumberStorage tickNumberStorage)
        {
            movementJoystick = forMovement;
            attackJoystick = forAttack;
            this.inputMessagesHistory = inputMessagesHistory;
            this.tickNumberStorage = tickNumberStorage;
        }

        public void Execute()
        {
            float attackAngle = float.NaN;
            bool useAbility = false;
            
            float x = movementJoystick.Horizontal;
            float y = movementJoystick.Vertical;
            
#if UNITY_EDITOR_WIN
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
#endif
            
            if (Math.Abs(attackJoystick.Horizontal) > 0.001f && Math.Abs(attackJoystick.Vertical) > 0.001f)
            {
                attackAngle = Mathf.Atan2(attackJoystick.Horizontal,  attackJoystick.Vertical) * Mathf.Rad2Deg;
                if (attackAngle < 0)
                {
                    attackAngle += 360;
                }
            }
            
            int? tickNumber = tickNumberStorage.GetCurrentTickNumber();
            if (tickNumber == null)
            {
                log.Error("Этот вызов не должен произойти. ");
                return;
            }

            
            InputMessageModel inputMessageModel = new InputMessageModel()
            {
                Angle = attackAngle,
                X = x,
                Y = y,
                UseAbility = useAbility,
                TickTimeMs = tickNumber.Value,
                TickNumber = tickNumber.Value
            };
            
            inputMessagesHistory.AddInput(inputMessageModel);
        }
    }
}