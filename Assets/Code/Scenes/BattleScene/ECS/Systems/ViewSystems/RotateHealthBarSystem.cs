// using System;
// using Code.Common.Logger;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class RotateHealthBarSystem : IExecuteSystem
//     {
//         private readonly IGroup<GameEntity> withHealthBar;
//         private readonly ILog log = LogManager.CreateLogger(typeof(RotateHealthBarSystem));
//
//         public RotateHealthBarSystem(Contexts contexts)
//         {
//             withHealthBar = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.View, GameMatcher.HealthInfo).NoneOf(GameMatcher.Hidden));
//         }
//
//         public void Execute()
//         {
//             try
//             {
//                 foreach (var e in withHealthBar)
//                 {
//                     var obj = e.healthInfo.value;
//
//                     obj.transform.rotation = Quaternion.identity;
//                 }
//             }
//             catch (Exception e)
//             {
//                 log.Error(e.Message + " " + e.StackTrace);
//             }
//         }
//     }
// }