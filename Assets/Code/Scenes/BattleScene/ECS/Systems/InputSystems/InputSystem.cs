using System;
using Code.Scenes.BattleScene.Experimental.Prediction;
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
        private readonly ISnapshotManager snapshotManager;
        private readonly IMatchTimeStorage matchTimeStorage;
        private readonly LastInputIdStorage lastInputIdStorage;
        private readonly ClientInputMessagesHistory clientInputMessagesHistory;
        private readonly ILog log = LogManager.CreateLogger(typeof(InputSystem));

        public InputSystem(Joystick forMovement, Joystick forAttack,
            ClientInputMessagesHistory clientInputMessagesHistory, ISnapshotManager snapshotManager,
            IMatchTimeStorage matchTimeStorage, LastInputIdStorage lastInputIdStorage)
        {
            movementJoystick = forMovement;
            attackJoystick = forAttack;
            this.clientInputMessagesHistory = clientInputMessagesHistory;
            this.snapshotManager = snapshotManager;
            this.matchTimeStorage = matchTimeStorage;
            this.lastInputIdStorage = lastInputIdStorage;
        }

        public void Execute()
        {
            float attackAngle = float.NaN;
            bool useAbility = false;
            
            float x = movementJoystick.Horizontal;
            float y = movementJoystick.Vertical;
            
#if UNITY_EDITOR_WIN
            float tolerance = 0.001f;
            if (Mathf.Abs(x) < tolerance && Mathf.Abs(y) < tolerance)
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

            float matchTime = matchTimeStorage.GetMatchTime();
            int? tickNumber = snapshotManager.GetCurrentTickNumber(matchTime);
            if (tickNumber == null)
            {
                throw new Exception($"Этот вызов не должен произойти. matchTime = {matchTime} ");
            }

            
            InputMessageModel inputMessageModel = new InputMessageModel()
            {
                Angle = attackAngle,
                X = x,
                Y = y,
                UseAbility = useAbility,
                TickTimeSec = matchTime,
                TickNumber = tickNumber.Value
            };
            
            uint lastInputId = clientInputMessagesHistory.AddInput(inputMessageModel);
            lastInputIdStorage.SetLastInputId(lastInputId);
        }
    }
}