// using Entitas;
// using System.Collections.Generic;
// using System.Linq;
// using Code.Scenes.BattleScene.Experimental.Approximation;
// using UnityEngine;
//
// //TODO тут вообще нужен lock?
//
// // ReSharper disable once CheckNamespace
// namespace Code.BattleScene.ECS.Systems
// {
//     public class UpdateRadiusSystem : IExecuteSystem, ITearDownSystem
//     {
//         private static Dictionary<ushort, float> radiuses = new Dictionary<ushort, float>(0);
//         private static bool wasProcessed;
//
//         private readonly GameContext gameContext;
//         private readonly IApproximator<float> approximator;
//         //readonly IGroup<ServerGameEntity> gameEntitiesGroup;
//
//         public UpdateRadiusSystem(Contexts contexts, IApproximator<float> radiusApproximator)
//         {
//             gameContext = contexts.serverGame;
//             approximator = radiusApproximator;
//             //gameEntitiesGroup = gameContext.GetGroup(ServerGameMatcher.Circle);
//         }
//
//         public static void SetNewRadiuses(Dictionary<ushort, float> newRadiuses)
//         {
//             lock (LockObj)
//             {
//                 if (wasProcessed)
//                 {
//                     radiuses = newRadiuses;
//                 }
//                 else
//                 {
//                     foreach (var pair in newRadiuses)
//                     {
//                         radiuses[pair.Key] = pair.Value;
//                     }
//                 }
//                 wasProcessed = false;
//             }
//         }
//
//         public void TearDown()
//         {
//             approximator.Clear();
//         }
//
//         public void Execute()
//         {
//             lock (LockObj)
//             {
//                 var radiuses = approximator.Get(Time.time);
//                 if (!wasProcessed)
//                 {
//                     approximator.Set(new Dictionary<ushort, float>(UpdateRadiusSystem.radiuses), Time.time);
//                     wasProcessed = true;
//                 }
//
//                 foreach (var radiusPair in radiuses)
//                 {
//                     var entity = gameContext.GetEntityWithId(radiusPair.Key);
//
//                     if (entity != null && entity.hasCircle)
//                     {
//                         entity.ReplaceCircle(radiusPair.Value);
//                         entity.isHidden = false;
//                         UpdateRadiusSystem.radiuses.Remove(radiusPair.Key);
//                     }
//                     else
//                     {
//                        //DebugLogWarning("Пришла информация об объекте, которого нет на карте: " + radiusPair.Key);
//                     }
//                 }
//
//                 //Зона всегда является объектом с id = 0
//                 if (radiuses.TryGetValue(0, out var zoneRadius))
//                 {
//                     if (gameContext.hasZoneInfo)
//                     {
//                         gameContext.zoneInfo.radius = zoneRadius;
//                     }
//                 }
//
//                 wasProcessed = UpdateRadiusSystem.radiuses.Count == 0;
//             }
//         }
//
//         private static readonly object LockObj = new object();
//     }
// }