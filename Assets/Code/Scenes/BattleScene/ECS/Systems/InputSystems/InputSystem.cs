using System;
using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.InputSystems
{
    public class InputSystem : IExecuteSystem
    {
        private readonly Joystick attackJoystick;
        private readonly Joystick movementJoystick;
        private readonly IMatchTimeStorage matchTimeStorage;
        private readonly ITickNumberStorage tickNumberStorage;
        private readonly ClientInputMessagesHistory clientInputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(InputSystem));

        public InputSystem(Joystick forMovement, Joystick forAttack,
            ClientInputMessagesHistory clientInputMessagesHistory, ITickNumberStorage tickNumberStorage,
            IMatchTimeStorage matchTimeStorage)
        {
            movementJoystick = forMovement;
            attackJoystick = forAttack;
            this.clientInputMessagesHistory = clientInputMessagesHistory;
            this.tickNumberStorage = tickNumberStorage;
            this.matchTimeStorage = matchTimeStorage;
        }

        public void Execute()
        {
            float attackAngle = float.NaN;
            bool useAbility = false;
            
            float x = movementJoystick.Horizontal;
            float y = movementJoystick.Vertical;
            
#if UNITY_EDITOR_WIN
            float tolerance = 0.001f;
            if (x < tolerance && y < tolerance)
            {
                x = Input.GetAxis("Horizontal");
                y = Input.GetAxis("Vertical");    
            }
#endif
            
            if (Math.Abs(attackJoystick.Horizontal) > tolerance && Math.Abs(attackJoystick.Vertical) > tolerance)
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
                TickTimeSec = matchTimeStorage.GetMatchTimeSec(),
                TickNumber = tickNumber.Value
            };
            
            clientInputMessagesHistory.AddInput(inputMessageModel);
        }
    }
}