// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class RenderTransformSystem : IExecuteSystem
//     {
//         private readonly IGroup<GameEntity> positionedGroup;
//
//         public RenderTransformSystem(Contexts contexts)
//         {
//             var matcher = GameMatcher.AllOf(GameMatcher.Transform2D, GameMatcher.View);
//             positionedGroup = contexts.game.GetGroup(matcher);
//         }
//
//         public void Execute()
//         {
//             try
//             {
//                 foreach (var gameEntity in positionedGroup)
//                 {
//                     var transform = gameEntity.view.gameObject.transform;
//                     Vector3 position = gameEntity.Transform2D.position;
//                     transform.position = position - Vector3.forward * (0.00001f * gameEntity.id.value);
//                     transform.rotation = Quaternion.AngleAxis(gameEntity.Transform2D.angle, Vector3.forward);
//                 }
//             }
//             catch
//             {
//                 // ignored
//             }
//         }
//     }
// }