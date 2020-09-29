using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    /// <summary>
    /// По вводу игрока поворачивает его с ограничением скорости
    /// </summary>
    public class RotationSystem : IExecuteSystem
    {
        private readonly ServerGameContext gameContext;
        private readonly IGroup<ServerInputEntity> inputGroup;
        private readonly ITickDeltaTimeStorage tickDeltaTimeStorage;
        private readonly PhysicsRotationManager physicsRotationManager;
        private readonly ILog log = LogManager.CreateLogger(typeof(RotationSystem));

        public RotationSystem(Contexts contexts, ITickDeltaTimeStorage tickDeltaTimeStorage)
        {
            this.tickDeltaTimeStorage = tickDeltaTimeStorage;
            gameContext = contexts.serverGame;
            inputGroup = contexts.serverInput.GetGroup(ServerInputMatcher
                .AllOf(ServerInputMatcher.PlayerInput, ServerInputMatcher.Attack));
            
            physicsRotationManager = new PhysicsRotationManager();
        }
        
        public void Execute()
        {
            foreach (var inputEntity in inputGroup)
            {
                if (!inputEntity.hasAttack)
                {
                    log.Error("Нет компонента атаки.");
                    continue;
                }

                float desiredAngle = inputEntity.attack.direction;
                if (float.IsNaN(desiredAngle))
                {
                    log.Info("Неправильное значение угла атаки.");
                    continue;
                }
                
                ushort playerId = inputEntity.playerInput.playerEntityId;
                ServerGameEntity playerEntity = gameContext.GetEntityWithPlayer(playerId);
                if (playerEntity == null)
                {
                    log.Error($"Нет такого игрока. {nameof(playerId)} {playerId}");
                    continue;
                }

                if (!playerEntity.hasRigidbody)
                {
                    log.Error($"Нет rigidbody");
                    continue;
                }
                
                if (float.IsNaN(desiredAngle))
                {
                    playerEntity.rigidbody.value.angularVelocity = Vector3.zero;
                    continue;
                }

                if (!playerEntity.hasAngularVelocity)
                {
                    log.Error("У сущности должен быть AngularVelocity");
                    continue;
                }

                float deltaTimeSec = tickDeltaTimeStorage.GetDeltaTimeSec();
                    
                // float angularVelocity = playerEntity.angularVelocity.value;
                float angularVelocity = 90;
                physicsRotationManager.ApplyRotation(playerEntity.rigidbody.value, desiredAngle, angularVelocity,
                    deltaTimeSec);
            }
        }
    }
}