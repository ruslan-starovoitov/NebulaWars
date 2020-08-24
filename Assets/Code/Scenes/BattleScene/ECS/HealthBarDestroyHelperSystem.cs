// using Entitas;
//
// namespace Code.Scenes.BattleScene.ECS
// {
//     /// <summary>
//     /// Если удаляется обьект с полосой жизни, то полосу нужно удалить тоже
//     /// </summary>
//     public class HealthBarDestroyHelperSystem : ICleanupSystem
//     {
//         private readonly IGroup<ServerGameEntity> needHealthBarDestroyGroup;
//
//         public HealthBarDestroyHelperSystem(Contexts contexts)
//         {
//             needHealthBarDestroyGroup = contexts.game.GetGroup(
//                 GameMatcher.AllOf(GameMatcher.Destroyed, GameMatcher.HealthBarParent));
//         }
//
//         public void Cleanup()
//         {
//             ServerGameEntity[] entities = needHealthBarDestroyGroup.GetEntities();
//             for (int index = 0; index < entities.Length; index++)
//             {
//                 ServerGameEntity entity = entities[index];
//                 entity.healthBarParent.healthBarEntity.isDestroyed = true;
//             }
//         }
//     }
// }