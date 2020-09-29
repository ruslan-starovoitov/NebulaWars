// using System.Collections.Generic;
// using Entitas;
//
// namespace Code.BattleScene.ECS.Systems
// {
//     public class UpdateTeamsSystem : IExecuteSystem
//     {
//         private static readonly object LockObj = new object();
//         private static Dictionary<ushort, byte> _teams = new Dictionary<ushort, byte>();
//         private static uint _lastMessageId;
//         private static bool WasProcessed = true;
//         private readonly GameContext gameContext;
//
//         public UpdateTeamsSystem(Contexts contexts)
//         {
//             gameContext = contexts.game;
//             _lastMessageId = 0;
//             WasProcessed = true;
//             _teams.Clear();
//         }
//
//         public static void SetNewTeams(uint messageId, Dictionary<ushort, byte> values)
//         {
//             lock (LockObj)
//             {
//                 if (WasProcessed)
//                 {
//                     _teams = values;
//                     WasProcessed = false;
//                 }
//                 else
//                 {
//                     if (messageId > _lastMessageId)
//                     {
//                         foreach (var pair in values)
//                         {
//                             _teams[pair.Key] = pair.Value;
//                         }
//
//                         _lastMessageId = messageId;
//                     }
//                     else
//                     {
//                         foreach (var pair in _teams)
//                         {
//                             values[pair.Key] = pair.Value;
//                         }
//
//                         _teams = values;
//                     }
//                 }
//             }
//         }
//
//         public void Execute()
//         {
//             lock (LockObj)
//             {
//                 if (WasProcessed) return;
//                 var teams = new Dictionary<ushort, byte>(_teams);
//
//                 foreach (var pair in teams)
//                 {
//                     var id = pair.Key;
//                     var entity = gameContext.GetEntityWithId(pair.Key);
//                     if (entity != null && entity.hasView && !entity.hasDelayedRecreation && !entity.hasDelayedSpawn)
//                     {
//                         entity.ReplaceTeam(pair.Value);
//
//                         _teams.Remove(id);
//                     }
//                 }
//
//                 if (_teams.Count == 0) WasProcessed = true;
//             }
//         }
//     }
// }