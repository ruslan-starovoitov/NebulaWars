// using System;
// using Code.Scenes.BattleScene.Experimental;
// using Entitas;
// using Plugins.submodules.SharedCode.Logger;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class RenderSmoothedTransformSystem : IExecuteSystem
//     {
//         private readonly IGroup<ServerGameEntity> positionedGroup;
//         private float SmoothTime = ClientTimeManager.TimeDelay;
//         private readonly ILog log = LogManager.CreateLogger(typeof(RenderSmoothedTransformSystem));
//
//         public RenderSmoothedTransformSystem(Contexts contexts)
//         {
//             var matcher = GameMatcher
//                 .AllOf(ServerGameMatcher.Position, GameMatcher.Direction, GameMatcher.View, GameMatcher.Speed);
//             positionedGroup = contexts.serverGame.GetGroup(matcher);
//         }
//
//         public void Execute()
//         {
//             try
//             {
//                 foreach (ServerGameEntity ServerGameEntity in positionedGroup)
//                 {
//                     var transform = ServerGameEntity.view.gameObject.transform;
//                     transform.localPosition =
//                         (Vector3) Vector2.SmoothDamp(transform.localPosition, ServerGameEntity.position.value,
//                             ref ServerGameEntity.speed.linear, SmoothTime) -
//                         Vector3.forward * (0.00001f * gameEntity.id.value);
//                     var newAngle = Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.z,
//                         gameEntity.direction.angle, ref gameEntity.speed.angular, SmoothTime);
//                     transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
//                 }
//             }
//             catch (Exception e)
//             {
//                 log.Error(e.Message+" "+e.StackTrace);
//             }
//         }
//     }
// }