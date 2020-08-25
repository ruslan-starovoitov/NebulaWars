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
        private readonly ServerInputContext inputContext;
        private readonly IGroup<ServerInputEntity> attackGroup;
        private readonly IGroup<ServerInputEntity> movementGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(JoysticksInputSystem));

        public JoysticksInputSystem(Contexts contexts, Joystick forMovement, Joystick forAttack)
        {
            inputContext = contexts.serverInput;
            movementJoystick = forMovement;
            attackJoystick = forAttack;
            movementGroup = inputContext.GetGroup(ServerInputMatcher.Movement);
            attackGroup = inputContext.GetGroup(ServerInputMatcher.Attack);
        }

        public void Execute()
        {
            if (movementGroup.count != 0)
            {
                log.Error("hasMovement");
                return;
            }

            if (attackGroup.count != 0)
            {
                log.Error("hasAttack");
                return;
            }

            var entity = inputContext.CreateEntity();
            float x = movementJoystick.Horizontal;
            float y = movementJoystick.Vertical;
            
#if UNITY_EDITOR_WIN
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
#endif
            
            Vector2 vector = new Vector2(x, y);
            entity.AddMovement(vector);
            

            if (Math.Abs(attackJoystick.Horizontal) > 0.001f && Math.Abs(attackJoystick.Vertical) > 0.001f)
            {
                float attackAngle = Mathf.Atan2(attackJoystick.Horizontal,  attackJoystick.Vertical) * Mathf.Rad2Deg;
                if (attackAngle < 0)
                {
                    attackAngle += 360;
                }
                entity.AddAttack(attackAngle);
            }
        }

        public void Cleanup()
        {
            var entities = movementGroup.GetEntities();
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].Destroy();
            }
            
            entities = attackGroup.GetEntities();
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].Destroy();
            }

            
        }
    }
}