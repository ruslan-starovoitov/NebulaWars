// using Entitas;
// using Entitas.Unity;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class DestroySystem : IExecuteSystem
//     {
//         private readonly IGroup<GameEntity> destroyedGroup;
//         private readonly List<GameEntity> buffer;
//         private const int predictedCapacity = 64;
//
//         public DestroySystem(Contexts contexts)
//         {
//             var matcher = GameMatcher.AllOf(GameMatcher.Destroyed, GameMatcher.View).NoneOf(GameMatcher.DestroyTimer);
//             destroyedGroup = contexts.game.GetGroup(matcher);
//             buffer = new List<GameEntity>(predictedCapacity);
//         }
//
//         public void Execute()
//         {
//             foreach (var e in destroyedGroup.GetEntities(buffer))
//             {
//                 var gameObject = e.view.gameObject;
//                 foreach (Transform childTransform in gameObject.transform)
//                 {
//                     var childLink = childTransform.gameObject.GetEntityLink();
//                     if (childLink != null && childLink.entity is GameEntity childEntity)
//                     {
//                         childEntity.RemoveParent();
//                         childTransform.SetParent(gameObject.transform.parent, true);
//                         if (childEntity.hasPosition)
//                         {
//                             childEntity.ReplacePosition(childTransform.localPosition);
//                         }
//                         else
//                         {
//                             childEntity.AddPosition(childTransform.localPosition);
//                         }
//
//                         if (childEntity.hasDirection)
//                         {
//                             childEntity.ReplaceDirection(childTransform.localRotation.eulerAngles.z);
//                         }
//                         else
//                         {
//                             childEntity.AddDirection(childTransform.localRotation.eulerAngles.z);
//                         }
//                     }
//                 }
//                 gameObject.Unlink();
//                 Object.Destroy(gameObject);
//                 e.RemoveView();
//
//                 if (e.hasDelayedSpawn)
//                 {
//                     var id = e.id.value;
//                     var newestViewType = e.viewType.id;
//                     var delayedSpawn = e.delayedSpawn;
//                     var position = e.position.value;
//                     var direction = e.direction.angle;
//                     var hasDeathSound = e.hasDeathSound;
//                     var deathSound = hasDeathSound ? e.deathSound.value : null;
//
//                     e.RemoveAllComponents();
//
//                     e.AddId(id);
//                     e.AddViewType(newestViewType);
//                     e.AddDelayedSpawn(delayedSpawn.typeId, delayedSpawn.positionX, delayedSpawn.positionY, delayedSpawn.direction, delayedSpawn.time);
//                     e.AddPosition(position);
//                     e.AddDirection(direction);
//                     if(hasDeathSound) e.AddSpawnSound(deathSound);
//                 }
//                 else
//                 {
//                     e.Destroy();
//                 }
//             }
//         }
//     }
// }