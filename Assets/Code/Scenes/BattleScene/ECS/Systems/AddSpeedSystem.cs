// using Entitas;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class AddSpeedSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly GameContext gameContext;
//
//         public AddSpeedSystem(Contexts contexts) : base(contexts.game)
//         {
//             gameContext = contexts.game;
//         }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             var matcher = GameMatcher.AllOf(GameMatcher.View).AnyOf(GameMatcher.Transform, GameMatcher.Hidden);
//             return context.CreateCollector(matcher);
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.hasTransform && entity.hasView && !entity.isDestroyed && !entity.hasSpeed && !entity.isHidden;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             foreach (GameEntity e in entities)
//             {
//                 e.AddSpeed(Vector2.zero, 0f);
//             }
//         }
//     }
// }