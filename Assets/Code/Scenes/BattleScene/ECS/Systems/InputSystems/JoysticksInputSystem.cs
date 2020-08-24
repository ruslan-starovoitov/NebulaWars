using System;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.InputSystems
{
    public class JoysticksInputSystem : IExecuteSystem, ICleanupSystem
    {
        private readonly Joystick attackJoystick;
        private readonly Joystick movementJoystick;
        private readonly InputContext inputContext;
        private readonly ILog log = LogManager.CreateLogger(typeof(JoysticksInputSystem));
        
        public JoysticksInputSystem(Contexts contexts, Joystick forMovement, Joystick forAttack)
        {
            inputContext = contexts.input;
            movementJoystick = forMovement;
            attackJoystick = forAttack;
        }

        public void Execute()
        {
            if (inputContext.hasMovement)
            {
                log.Error("hasMovement");
                return;
            }

            if (inputContext.hasAttack)
            {
                log.Error("hasAttack");
                return;
            }

            inputContext.SetMovement(movementJoystick.Horizontal, movementJoystick.Vertical);
#if UNITY_EDITOR_WIN
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector2 inputVector = new Vector2(x, y);
            if (inputVector.sqrMagnitude > 0f)
            {
                inputContext.ReplaceMovement(inputVector.x, inputVector.y);
            }
#endif
            if (Math.Abs(attackJoystick.Horizontal) > 0.001 && Math.Abs(attackJoystick.Vertical) > 0.001)
            {
                float attackAngle = Mathf.Atan2(attackJoystick.Horizontal,  attackJoystick.Vertical) * Mathf.Rad2Deg;
                if (attackAngle < 0)
                {
                    attackAngle += 360;
                }
                inputContext.SetAttack(attackAngle);
            }
        }

        public void Cleanup()
        {
            inputContext.RemoveMovement();
            if (inputContext.hasAttack)
            {
                inputContext.RemoveAttack();
            }
        }
    }
}