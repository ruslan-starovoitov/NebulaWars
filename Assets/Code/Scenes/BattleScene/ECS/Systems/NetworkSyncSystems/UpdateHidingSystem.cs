// using System.Collections.Generic;
// using Entitas;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
// {
//     public class UpdateHidingSystem : IExecuteSystem
//     {
//         private static readonly object LockObj = new object();
//         private static readonly HashSet<ushort> _hides = new HashSet<ushort>();
//         private readonly GameContext gameContext;
//
//         public UpdateHidingSystem(Contexts contexts)
//         {
//             gameContext = contexts.serverGame;
//             lock (LockObj)
//             {
//                 _hides.Clear();
//             }
//         }
//
//         public static void SetNewHides(uint messageId, IEnumerable<ushort> newHides)
//         {
//             lock (LockObj)
//             {
//                 _hides.UnionWith(newHides);
//             }
//         }
//
//         public void Execute()
//         {
//             lock (LockObj)
//             {
//                 foreach (var id in _hides)
//                 {
//                     var entity = gameContext.GetEntityWithId(id);
//
//                     if (entity != null)
//                     {
//                         entity.isHidden = true;
//                         if(entity.hasSpeed) entity.RemoveSpeed();
//                     }
//                 }
//
//                 _hides.Clear();
//             }
//         }
//     }
// }