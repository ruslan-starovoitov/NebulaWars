// using Code.Scenes.BattleScene.ECS.Components.Game.TimerComponents;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class DelayedRecreationSystem : BaseTimerSubtractionSystem<DelayedRecreationComponent>
//     {
//         protected override int PredictedCapacity { get; } = 64;
//
//         public DelayedRecreationSystem(Contexts contexts) : base(contexts)
//         { }
//
//         protected override void OnTimeExpired(GameEntity entity)
//         {
//             var recreation = entity.delayedRecreation;
//
//             if (entity.hasDelayedSpawn)
//             {
//                 Debug.LogWarning($"Сущность с id {entity.id.value} уже имеет отложенный спаун, выполняется пропуск кадра отнимания времени компонента.");
//                 entity.delayedSpawn.time = 0f;
//                 if (entity.hasDestroyTimer) entity.destroyTimer.time = 0f;
//                 return;
//             }
//             else
//             {
//                 entity.AddDelayedSpawn(recreation.typeId, recreation.positionX, recreation.positionY, recreation.direction, 0f);
//             }
//             entity.isDestroyed = true;
//
//             base.OnTimeExpired(entity);
//
//             if (entity.hasManyDelayedRecreations)
//             {
//                 var queue = entity.manyDelayedRecreations.components;
//                 var newComponent = queue.Dequeue();
//                 entity.AddComponent(GameComponentsLookup.DelayedRecreation, newComponent);
//
//                 if(queue.Count == 0) entity.RemoveManyDelayedRecreations();
//             }
//         }
//     }
// }