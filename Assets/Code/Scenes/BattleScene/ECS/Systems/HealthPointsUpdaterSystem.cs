// using Code.Common.Logger;
// using Entitas;
// using System;
// using System.Collections.Generic;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class HealthPointsUpdaterSystem : IExecuteSystem
//     {
//         private static readonly object LockObj = new object();
//         private static volatile Dictionary<ushort, ushort> healthPoints;
//         private static volatile bool wasChanged;
//         private readonly GameContext gameContext;
//         private readonly HealthAndShieldPointsDisplayingSystem playerPointsSystem;
//         private readonly ILog log = LogManager.CreateLogger(typeof(HealthPointsUpdaterSystem));
//
//         public HealthPointsUpdaterSystem(Contexts contexts, HealthAndShieldPointsDisplayingSystem hpDisplayingSystem)
//         {
//             if(hpDisplayingSystem == null)
//                 throw new Exception($"{nameof(HealthPointsUpdaterSystem)} {nameof(hpDisplayingSystem)} was null");
//
//             gameContext = contexts.game;
//             playerPointsSystem = hpDisplayingSystem;
//             healthPoints = new Dictionary<ushort, ushort>(0);
//             wasChanged = false;
//         }
//
//         public static void SetHealthPoints(Dictionary<ushort, ushort> healthPointsArg)
//         {
//             lock (LockObj)
//             {
//                 if (wasChanged)
//                 {
//                     foreach (var oldPair in healthPoints)
//                     {
//                         if(!healthPointsArg.ContainsKey(oldPair.Key)) healthPointsArg.Add(oldPair.Key, oldPair.Value);
//                     }
//                 }
//                 healthPoints = healthPointsArg;
//                 wasChanged = true;
//             }
//         }
//
//         public void Execute()
//         {
//             lock (LockObj)
//             {
//                 if (!wasChanged) return;
//                 var dict = new Dictionary<ushort, ushort>(healthPoints);
//
//                 foreach (var pair in dict)
//                 {
//                     var entity = gameContext.GetEntityWithId(pair.Key);
//
//                     if (entity == null || entity.hasDelayedRecreation) continue;
//
//                     if (entity.isCurrentPlayer)
//                     {
//                         playerPointsSystem.SetHealthPoints(pair.Value);
//                     }
//                     else
//                     {
//                         if (entity.hasHealth)
//                         {
//                             entity.ReplaceHealth(pair.Value, entity.health.maximum);
//                         }
//                         else
//                         {
//                             entity.AddHealth(pair.Value, pair.Value);
//                         }
//                         if (!entity.hasPlayer && !entity.isShield) entity.ReplaceHealthBarFading(1f);
//                     }
//
//                     healthPoints.Remove(pair.Key);
//                 }
//                 wasChanged = healthPoints.Count > 0;
//             }
//         }
//     }
// }