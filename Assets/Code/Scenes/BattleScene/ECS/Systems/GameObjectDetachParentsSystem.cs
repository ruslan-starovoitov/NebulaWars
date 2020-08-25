// using System.Collections.Generic;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class GameObjectDetachParentsSystem : ReactiveSystem<ServerGameEntity>
//     {
//         readonly Transform viewContainer;
//
//         public GameObjectDetachParentsSystem(Contexts contexts, GameObject gameViews) : base(contexts.game)
//         {
//             viewContainer = gameViews.transform;
//         }
//
//         protected override ICollector<ServerGameEntity> GetTrigger(IContext<ServerGameEntity> context)
//         {
//             return context.CreateCollector(ServerGameMatcher.Parent.Removed());
//         }
//
//         protected override bool Filter(ServerGameEntity entity)
//         {
//             return entity.hasView && !entity.hasParent;
//         }
//
//         protected override void Execute(List<ServerGameEntity> entities)
//         {
//             foreach (var e in entities)
//             {
//                 e.view.gameObject.transform.SetParent(viewContainer, true);
//             }
//         }
//     }
// }