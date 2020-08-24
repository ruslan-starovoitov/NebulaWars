// using System.Collections.Generic;
// using System.Linq;
// using Code.Scenes.BattleScene.Udp.MessageProcessing.Synchronizers;
// using Entitas;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
// {
//     public class DetachParentsSystem : IExecuteSystem
//     {
//         private static readonly object LockObj = new object();
//         private static HashSet<ushort> detaches;
//         private readonly ParentsNetworkSynchronizer _synchronizer = ParentsNetworkSynchronizer.Instance;
//         private readonly GameContext gameContext;
//
//         public DetachParentsSystem(Contexts contexts)
//         {
//             gameContext = contexts.game;
//             detaches = new HashSet<ushort>();
//         }
//
//         public static void SetNewDetaches(HashSet<ushort> newDetaches)
//         {
//             lock (LockObj)
//             {
//                 detaches = newDetaches;
//             }
//         }
//
//         public void Execute()
//         {
//             lock (LockObj)
//             {
//                 if (_synchronizer.HashSetWasProcessed) return;
//
//                 var newDetaches = detaches.ToArray();
//
//                 foreach (var id in newDetaches)
//                 {
//                     var entity = gameContext.GetEntityWithId(id);
//
//                     if (entity != null && entity.hasParent)
//                     {
//                         entity.RemoveParent();
//                         detaches.Remove(id);
//                     }
//                 }
//
//                 if(detaches.Count == 0) _synchronizer.HashSetWasProcessed = true;
//             }
//         }
//     }
// }