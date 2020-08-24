// using System.Collections.Generic;
// using Code.Scenes.BattleScene.Udp.MessageProcessing.Synchronizers;
// using Entitas;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
// {
//     public class UpdateParentsSystem : IExecuteSystem, ITearDownSystem
//     {
//         private static readonly object LockObj = new object();
//         private static Dictionary<ushort, ushort> parents;
//         private readonly ParentsNetworkSynchronizer _synchronizer = ParentsNetworkSynchronizer.Instance;
//         private readonly GameContext gameContext;
//
//         public UpdateParentsSystem(Contexts contexts)
//         {
//             gameContext = contexts.game;
//         }
//
//         public static void SetNewParents(Dictionary<ushort, ushort> newParents)
//         {
//             lock (LockObj)
//             {
//                 parents = newParents;
//             }
//         }
//
//         public void TearDown()
//         {
//             _synchronizer.Clear();
//         }
//
//         public void Execute()
//         {
//             lock (LockObj)
//             {
//                 if (_synchronizer.DictionaryWasProcessed) return;
//                 var newParents = new Dictionary<ushort, ushort>(parents);
//
//                 foreach (var pair in newParents)
//                 {
//                     var entity = gameContext.GetEntityWithId(pair.Key);
//                     if (entity != null)
//                     {
//                         if (entity.hasParent)
//                         {
//                             entity.ReplaceParent(pair.Value);
//                         }
//                         else
//                         {
//                             entity.AddParent(pair.Value);
//                         }
//
//                         parents.Remove(pair.Key);
//                     }
//                 }
//
//                 if (parents.Count == 0) _synchronizer.DictionaryWasProcessed = true;
//             }
//         }
//     }
// }