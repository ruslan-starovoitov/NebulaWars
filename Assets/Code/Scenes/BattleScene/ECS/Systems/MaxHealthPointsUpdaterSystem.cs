// using Code.Common.Logger;
// using Entitas;
// using System;
// using System.Collections.Generic;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class MaxHealthPointsUpdaterSystem : IExecuteSystem
//     {
//         private static readonly object LockObj = new object();
//         private static volatile Dictionary<ushort, ushort> maxHealthPoints;
//         private static volatile bool wasChanged;
//         private readonly GameContext gameContext;
//         private readonly HealthAndShieldPointsDisplayingSystem playerPointsSystem;
//         private readonly ILog log = LogManager.CreateLogger(typeof(MaxHealthPointsUpdaterSystem));
//
//         public MaxHealthPointsUpdaterSystem(Contexts contexts, HealthAndShieldPointsDisplayingSystem hpDisplayingSystem)
//         {
//             if (hpDisplayingSystem == null)
//                 throw new Exception($"{nameof(HealthPointsUpdaterSystem)} {nameof(hpDisplayingSystem)} was null");
//
//             gameContext = contexts.game;
//             playerPointsSystem = hpDisplayingSystem;
//             maxHealthPoints = new Dictionary<ushort, ushort>(0);
//             wasChanged = false;
//         }
//
//         public static void SetMaxHealthPoints(Dictionary<ushort, ushort> maxHealthPointsArg)
//         {
//             lock (LockObj)
//             {
//                 if (wasChanged)
//                 {
//                     foreach (var oldPair in maxHealthPoints)
//                     {
//                         if (!maxHealthPointsArg.ContainsKey(oldPair.Key)) maxHealthPointsArg.Add(oldPair.Key, oldPair.Value);
//                     }
//                 }
//                 maxHealthPoints = maxHealthPointsArg;
//                 wasChanged = true;
//             }
//         }
//
//         public void Execute()
//         {
//             lock (LockObj)
//             {
//                 if (!wasChanged) return;
//                 var dict = new Dictionary<ushort, ushort>(maxHealthPoints);
//
//                 foreach (var pair in dict)
//                 {
//                     var entity = gameContext.GetEntityWithId(pair.Key);
//
//                     if (entity == null || entity.hasDelayedRecreation) continue;
//
//                     if (entity.isCurrentPlayer)
//                     {
//                         playerPointsSystem.SetMaxHealthPoints(pair.Value);
//                     }
//                     else
//                     {
//                         if (entity.hasHealth)
//                         {
//                             entity.ReplaceHealth(entity.health.current, pair.Value);
//                         }
//                         else
//                         {
//                             entity.AddHealth(pair.Value, pair.Value);
//                         }
//                         if (!entity.hasPlayer && !entity.isShield) entity.ReplaceHealthBarFading(1f);
//                     }
//
//                     maxHealthPoints.Remove(pair.Key);
//                 }
//                 wasChanged = maxHealthPoints.Count > 0;
//             }
//         }
//     }
// }