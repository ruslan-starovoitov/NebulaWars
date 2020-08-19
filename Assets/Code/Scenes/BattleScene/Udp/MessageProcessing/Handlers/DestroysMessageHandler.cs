// using Code.Scenes.BattleScene.Udp.MessageProcessing.Synchronizers;
// using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
//
//
// namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
// {
//     public class DestroysMessageHandler : MessageHandler<DestroysMessage>
//     {
//         private readonly ParentsNetworkSynchronizer _parentsSynchronizer = ParentsNetworkSynchronizer.Instance;
//
//         protected override void Handle(in DestroysMessage message, uint messageId, bool needResponse)
//         {
//             UpdateDestroysSystem.SetNewDestroys(message.DestroyedIds);
//
//             _parentsSynchronizer.Remove(message.DestroyedIds);
//         }
//     }
// }