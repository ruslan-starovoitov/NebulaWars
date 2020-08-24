// using System;
// using Entitas;
// using UnityEngine;
//
// //TODO: возможно, стоит сделать эту систему реактивной
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public sealed class GlobalTransformSystem : IExecuteSystem, ICleanupSystem
//     {
//         private readonly GameContext gameContext;
//         private readonly IGroup<ServerGameEntity> positionedGroup;
//
//         public GlobalTransformSystem(Contexts contexts)
//         {
//             gameContext = contexts.game;
//             positionedGroup = gameContext.GetGroup(GameMatcher.Position);
//         }
//
//         public void Execute()
//         {
//             foreach (var e in positionedGroup)
//             {
//                 if (e.hasTransform) continue;
//                 AddTransform(e);
//             }
//         }
//
//         public void Cleanup()
//         {
//             foreach (var e in positionedGroup)
//             {
//                 if (e.hasTransform)
//                 {
//                     e.RemoveTransform();
//                 }
//             }
//         }
//
//         private void AddTransform(ServerGameEntity entity)
//         {
//             var position = (Vector2)entity.position.value;
//             var angle = entity.hasDirection ? entity.direction.angle : 0f;
//
//             if (entity.hasParent)
//             {
//                 var parent = gameContext.GetEntityWithId(entity.parent.id);
//                 if(parent == null) return; // где-то потеряли пакет
//                 if (!parent.hasTransform) AddTransform(parent);
//                 if (!parent.hasTransform) return; // снова пакет потеряли
//
//                 var parentAngle = parent.transform.angle;
//                 if (Math.Abs(parentAngle) > 0.01f)
//                 {
//                     Rotate(ref position, parentAngle);
//                     angle += parentAngle;
//                     // Кажется, одного отнимания будет достаточно
//                     if (angle >= 360f) angle -= 360f;
//                 }
//
//                 position += parent.transform.position;
//             }
//
//             entity.AddTransform(position, angle);
//         }
//
//         private static void Rotate(ref Vector2 vector, float angle)
//         {
//             var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
//             var cos = Mathf.Cos(angle * Mathf.Deg2Rad);
//             var newX = cos * vector.x - sin * vector.y;
//             var newY = sin * vector.x + cos * vector.y;
//             vector.x = newX;
//             vector.y = newY;
//         }
//     }
// }
