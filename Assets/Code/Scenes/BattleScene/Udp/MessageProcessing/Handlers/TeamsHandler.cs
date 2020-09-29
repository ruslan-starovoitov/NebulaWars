// using Code.BattleScene.ECS.Systems;
// using Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;
//
// namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
// {
//     public class TeamsHandler : MessageHandler<TeamsMessage>
//     {
//         protected override void Handle(in TeamsMessage message, uint messageId, bool needResponse)
//         {
//             UpdateTeamsSystem.SetNewTeams(messageId, message.Teams);
//         }
//     }
// }