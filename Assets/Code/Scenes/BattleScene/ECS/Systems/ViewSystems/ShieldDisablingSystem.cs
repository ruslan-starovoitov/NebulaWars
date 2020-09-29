// using System.Collections.Generic;
// using Entitas;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class ShieldDisablingSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly GameContext gameContext;
//         private readonly HealthAndShieldPointsDisplayingSystem displayingSystem;
//
//         public ShieldDisablingSystem(Contexts contexts, HealthAndShieldPointsDisplayingSystem shieldPointsDisplayingSystem) : base(contexts.game)
//         {
//             gameContext = contexts.game;
//             displayingSystem = shieldPointsDisplayingSystem;
//         }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             return context.CreateCollector(GameMatcher.Destroyed.Added());
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.isShield && entity.isDestroyed;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             foreach (var e in entities)
//             {
//                 if (e.isCurrentShield)
//                 {
//                     displayingSystem.SetShieldPoints(0);
//                     displayingSystem.SetMaxShieldPoints(0);
//                 }
//                 else if(e.hasHealthInfo)
//                 {
//                     e.healthInfo.value.HideHealthBar();
//                 }
//             }
//         }
//     }
// }