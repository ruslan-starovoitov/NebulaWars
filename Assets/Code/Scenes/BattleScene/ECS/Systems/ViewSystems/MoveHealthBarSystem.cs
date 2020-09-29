// using System;
// using Code.Common.Logger;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class MoveHealthBarSystem : IExecuteSystem
//     {
//         private readonly GameContext gameContext;
//         private readonly IGroup<GameEntity> withHealthBar;
//         private readonly ILog log = LogManager.CreateLogger(typeof(MoveHealthBarSystem));
//
//         public MoveHealthBarSystem(Contexts contexts)
//         {
//             gameContext = contexts.game;
//             withHealthBar = gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.View,
//                     GameMatcher.HealthInfo,
//                     GameMatcher.InfoDistance)
//                 .NoneOf(GameMatcher.Hidden, GameMatcher.Shield));
//         }
//
//         public void Execute()
//         {
//             try
//             {
//                 var playerEntity = gameContext.currentPlayerEntity;
//                 if (playerEntity == null || !playerEntity.hasView) return;
//                 var playerPosition = playerEntity.view.gameObject.transform.position;
//
//                 foreach (var e in withHealthBar)
//                 {
//                     var currentTransform = e.view.gameObject.transform;
//                     var nickDist = e.infoDistance.value;
//                     var obj = e.healthInfo.value;
//
//                     var currentPosition = currentTransform.position;
//                     var direction = -(currentPosition - playerPosition).normalized;
//
//                     obj.transform.localPosition = Quaternion.Inverse(currentTransform.rotation) * direction * nickDist;
//                 }
//             }
//             catch (Exception e)
//             {
//                 log.Error(e.Message + " " + e.StackTrace);
//             }
//         }
//     }
// }