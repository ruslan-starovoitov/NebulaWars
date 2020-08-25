// using System.Collections.Generic;
// using Entitas;
// using Entitas.Unity;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class AddViewSystem : ReactiveSystem<ServerGameEntity>
//     {
//         readonly Transform viewContainer;
//
//         public AddViewSystem(Contexts contexts, GameObject gameViews) : base(contexts.game)
//         {
//             viewContainer = gameViews.transform;
//         }
//
//         protected override ICollector<ServerGameEntity> GetTrigger(IContext<ServerGameEntity> context)
//         {
//             var matcher = GameMatcher.AnyOf(ServerGameMatcher.Sprite, GameMatcher.CircleLine, GameMatcher.StraightLine);
//             return context.CreateCollector(matcher);
//         }
//
//         protected override bool Filter(ServerGameEntity entity)
//         {
//             return (entity.hasSprite || entity.hasCircleLine || entity.hasStraightLine) && !entity.hasView;
//         }
//
//         protected override void Execute(List<ServerGameEntity> entities)
//         {
//             foreach (ServerGameEntity e in entities)
//             {
//                 GameObject go = new GameObject("Game ViewComponent");
//                 go.transform.localScale = Vector3.one;
//                 go.transform.SetParent(viewContainer, false);
//                 e.AddView(go);
//                 go.Link(e);
//             }
//         }
//     }
// }