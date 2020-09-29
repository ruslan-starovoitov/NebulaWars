using System;
using System.Collections.Generic;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    /// <summary>
    /// Во набору вводов игрока создаёт средний вектор скорости и применяет его к rigidbody игрока.
    /// </summary>
    public class MoveSystem : IExecuteSystem
    {
        private readonly ServerGameContext gameContext;
        private readonly IGroup<ServerInputEntity> inputGroup;
        private readonly PhysicsVelocityManager physicsVelocityManager;
        private readonly Vector3Utils vector3Utils = new Vector3Utils();
        private readonly ILog log = LogManager.CreateLogger(typeof(MoveSystem));
        private readonly ILastProcessedInputIdStorage lastProcessedInputIdStorage;

        public MoveSystem(Contexts contexts, PhysicsVelocityManager physicsVelocityManager)
        {
            this.physicsVelocityManager = physicsVelocityManager;
            this.lastProcessedInputIdStorage = lastProcessedInputIdStorage;
            gameContext = contexts.serverGame;
            var matcher = ServerInputMatcher
                .AllOf(ServerInputMatcher.Movement, ServerInputMatcher.PlayerInput);
            inputGroup = contexts.serverInput.GetGroup(matcher);
        }
        
        public void Execute()
        {
            Dictionary<ushort, List<Vector3>> playerDirection = new Dictionary<ushort, List<Vector3>>();
            foreach (var inputEntity in inputGroup)
            {
                Vector2 playerJoystickInput = inputEntity.movement.value;
                ushort playerId = inputEntity.playerInput.playerEntityId;
                if (!playerDirection.ContainsKey(playerId))
                {
                    playerDirection.Add(playerId, new List<Vector3>());
                }
                
                Vector3 inputVector = new Vector3(playerJoystickInput.x, 0, playerJoystickInput.y);
                playerDirection[playerId].Add(inputVector);
            }

            foreach (var pair in playerDirection)
            {
                ushort playerId = pair.Key;
                ServerGameEntity playerEntity = gameContext.GetEntityWithPlayer(playerId);
                if (playerEntity == null)
                {
                    string message = $"Пришло сообщение о движении от игрока, которого нет в комнате. " +
                                     $"Данные игнорируются. {nameof(playerId)} {playerId}";
                    log.Error(message);
                    return;
                }
            
                if (!playerEntity.hasRigidbody)
                {
                    log.Error("Нет rigidbody");
                    continue;
                }

                Rigidbody rigidbody = playerEntity.rigidbody.value; 
                if (rigidbody.velocity != Vector3.zero)
                {
                    var vector = rigidbody.velocity;
                    throw new Exception($"Не нулевая скорость {vector.x} {vector.y} {vector.z}");
                }
                
                float maxSpeed = 10;
                List<Vector3> inputValues = pair.Value;
                Vector3 averageInputVector = vector3Utils.GetVelocityVector(inputValues);
                physicsVelocityManager.ApplyVelocity(rigidbody, averageInputVector, maxSpeed);
            }    
        }
    }
}