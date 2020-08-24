// using System.Collections.Generic;
// using Code.BattleScene.ECS.Systems;
// using Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems;
//
// namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Synchronizers
// {
//     public class ParentsNetworkSynchronizer : AbstractNetworkSynchronizer<ParentsNetworkSynchronizer, ushort, ushort>
//     {
//         public override void HandleDictionary(uint messageId, Dictionary<ushort, ushort> dictionary)
//         {
//             base.HandleDictionary(messageId, dictionary);
//             UpdateParentsSystem.SetNewParents(Dictionary);
//         }
//
//         public override void HandleSet(uint messageId, IEnumerable<ushort> set)
//         {
//             base.HandleSet(messageId, set);
//             DetachParentsSystem.SetNewDetaches(HashSet);
//         }
//     }
// }