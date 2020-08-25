// using Entitas;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class AddSpeedSystem : ReactiveSystem<ServerGameEntity>
//     {
//         private readonly GameContext gameContext;
//
//         public AddSpeedSystem(Contexts contexts) : base(contexts.game)
//         {
//             gameContext = contexts.serverGame;
//         }
//
//         protected override ICollector<ServerGameEntity> GetTrigger(IContext<ServerGameEntity> context)
//         {
//             var matcher = GameMatcher.AllOf(ServerGameMatcher.View).AnyOf(ServerGameMatcher.Transform, GameMatcher.Hidden);
//             return context.CreateCollector(matcher);
//         }
//
//         protected override bool Filter(ServerGameEntity entity)
//         {
//             return entity.hasTransform && entity.hasView && !entity.isDestroyed && !entity.hasSpeed && !entity.isHidden;
//         }
//
//         protected override void Execute(List<ServerGameEntity> entities)
//         {
//             foreach (ServerGameEntity e in entities)
//             {
//                 e.AddSpeed(Vector2.zero, 0f);
//             }
//         }
//     }
// }