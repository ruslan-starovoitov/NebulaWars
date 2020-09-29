// using System.Collections.Generic;
// using Entitas;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class CurrentShieldSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly GameContext gameContext;
//
//         public CurrentShieldSystem(Contexts contexts) : base(contexts.game)
//         {
//             gameContext = contexts.game;
//         }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             var matcher = GameMatcher.AnyOf(GameMatcher.Shield, GameMatcher.Parent);
//             return context.CreateCollector(matcher);
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.isShield && entity.hasParent;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             var currentPlayer = gameContext.currentPlayerEntity;
//             if (currentPlayer == null) return;
//             foreach (GameEntity e in entities)
//             {
//                 if (e.parent.id == currentPlayer.id.value)
//                 {
//                     if (gameContext.isCurrentShield) gameContext.currentShieldEntity.isCurrentShield = false;
//                     e.isCurrentShield = true;
//                 }
//             }
//         }
//     }
// }