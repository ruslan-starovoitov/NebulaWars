// using System;
// using System.Collections.Generic;
// using Code.Common.Logger;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class HealthBarFadingSystem : IExecuteSystem
//     {
//         private const float TimeToFade = 2.5f;
//         private const float PerSecondFade = 1f / TimeToFade;
//         private const int PredictedCapacity = 32;
//         private readonly List<GameEntity> buffer;
//         private readonly GameContext gameContext;
//         private readonly IGroup<GameEntity> withFadingHealthBar;
//         private readonly ILog log = LogManager.CreateLogger(typeof(MoveHealthBarSystem));
//
//         public HealthBarFadingSystem(Contexts contexts)
//         {
//             gameContext = contexts.game;
//             buffer = new List<GameEntity>(PredictedCapacity);
//             withFadingHealthBar = gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.HealthBarFading,
//                 GameMatcher.HealthInfo));
//         }
//
//         public void Execute()
//         {
//             try
//             {
//                 var delta = PerSecondFade * Time.deltaTime;
//                 foreach (var e in withFadingHealthBar.GetEntities(buffer))
//                 {
//                     var newPercentage = e.healthBarFading.percentage - delta;
//                     var obj = e.healthInfo.value;
//                     if (newPercentage > 0f)
//                     {
//                         e.ReplaceHealthBarFading(newPercentage);
//                     }
//                     else
//                     {
//                         e.RemoveHealthBarFading();
//                     }
//                     obj.SetTransparency(newPercentage);
//                     obj.SaveChanges();
//                 }
//             }
//             catch (Exception e)
//             {
//                 log.Error(e.Message + " " + e.StackTrace);
//             }
//         }
//     }
// }