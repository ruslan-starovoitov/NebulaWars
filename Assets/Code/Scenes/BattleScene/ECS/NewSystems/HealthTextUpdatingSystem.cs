// using System.Collections.Generic;
// using Entitas;
// using Plugins.submodules.SharedCode.Logger;
//
// namespace Code.Scenes.BattleScene.ECS.NewSystems
// {
//     public class HealthTextUpdatingSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly ILog log = LogManager.CreateLogger(typeof(HealthTextUpdatingSystem));
//         
//         public HealthTextUpdatingSystem(Contexts contexts)
//             :base(contexts.game)
//         {
//         }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             return context.CreateCollector(GameMatcher.Health.Added(), GameMatcher.MaxHealth.Added());
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.hasHealth && entity.hasMaxHealth && entity.hasHealthBarParent;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             foreach (var entity in entities)
//             {
//                 int health = entity.health.value;
//                 int maxHealth = entity.maxHealth.value;
//                 string value = $"{health}/{maxHealth}";
//                 entity.healthBarParent.healthBarEntity.healthBar.healthPoints.text = value;
//                 // log.Debug("Изменение хп "+value);
//             }
//         }
//     }
// }