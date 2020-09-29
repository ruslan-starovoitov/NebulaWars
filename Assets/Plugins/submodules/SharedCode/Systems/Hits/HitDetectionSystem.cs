using Entitas;
using Entitas.Unity;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Hits
{
    /// <summary>
    /// Обнаруживает столкновение пуль
    /// </summary>
    public class HitDetectionSystem : LagCompensationSystem
    {
        private readonly ServerGameContext gameContext;
        private readonly PhysicsRaycaster physicsRaycaster;
        private readonly ITickDeltaTimeStorage tickDeltaTimeStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(HitDetectionSystem));

        public HitDetectionSystem(Contexts contexts, PhysicsRaycaster physicsRaycaster,
            ITickDeltaTimeStorage tickDeltaTimeStorage)
        {
            this.physicsRaycaster = physicsRaycaster;
            this.tickDeltaTimeStorage = tickDeltaTimeStorage;
            gameContext = contexts.serverGame;
        }
        
        // public override GameState PastState { get; set; }
        // public override GameState PresentState { get; set; }

        public override void Execute(ServerGameEntity damageEntity)
        {
            if (!damageEntity.hasTransform)
            {
                return;
            }

            if (!damageEntity.hasDamage)
            {
                return;
            }

            var testTransform = damageEntity.transform.value;
            if (testTransform == null)
            {
                log.Error("Transform пуст");
                return;
            }
            Vector3 currentPosition = damageEntity.transform.value.position;
            Vector3 direction = damageEntity.transform.value.rotation * Vector3.forward;
            Vector3 velocity = damageEntity.rigidbody.value.velocity * tickDeltaTimeStorage.GetDeltaTimeSec();

            //Есть столкновение?
            bool collisionOccurred = physicsRaycaster
                .Raycast(currentPosition, direction, velocity.magnitude, out RaycastHit raycastHit); 
            if (!collisionOccurred)
            {
                return;
            }
            
            EntityLink entityLink = raycastHit.transform.gameObject.GetEntityLink();
            ServerGameEntity targetEntity = (ServerGameEntity) entityLink.entity;
            if (targetEntity == null)
            {
                return;
            }
                
            ushort entityId = targetEntity.id.value;
            //Проверка попадания по самому себе
            if (damageEntity.parentWarship.entity.id.value == entityId)
            {
                log.Error($"Попадание по самому себе parentId = {entityId}");
                return;
            }

            ServerGameEntity hitEntity = gameContext.CreateEntity();
            hitEntity.AddHit(damageEntity, targetEntity);
        }
    }
}