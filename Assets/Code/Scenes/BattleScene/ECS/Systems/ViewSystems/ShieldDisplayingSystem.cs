// using System.Collections.Generic;
// using Entitas;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
// {
//     public class ShieldDisplayingSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly GameContext gameContext;
//         private readonly HealthAndShieldPointsDisplayingSystem displayingSystem;
//
//         public ShieldDisplayingSystem(Contexts contexts, HealthAndShieldPointsDisplayingSystem shieldPointsDisplayingSystem) : base(contexts.game)
//         {
//             gameContext = contexts.game;
//             displayingSystem = shieldPointsDisplayingSystem;
//         }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             return context.CreateCollector(GameMatcher.Health);
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.isCurrentShield && entity.hasHealth;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             foreach (GameEntity e in entities)
//             {
//                 displayingSystem.SetShieldPoints(e.health.current);
//                 displayingSystem.SetMaxShieldPoints(e.health.maximum);
//             }
//         }
//     }
// }