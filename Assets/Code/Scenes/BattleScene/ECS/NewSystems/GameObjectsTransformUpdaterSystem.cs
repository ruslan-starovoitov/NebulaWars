// using Entitas;
// using Plugins.submodules.SharedCode.Logger;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.NewSystems
// {
//     /// <summary>
//     /// Задаёт всем обьектам позициб и поворот по данным из памяти
//     /// </summary>
//     public class GameObjectsTransformUpdaterSystem : IExecuteSystem
//     {
//         private readonly IGroup<ServerGameEntity> viewAndTransformGroup;
//         private readonly ILog log = LogManager.CreateLogger(typeof(GameObjectsTransformUpdaterSystem));
//
//         public GameObjectsTransformUpdaterSystem(Contexts contexts)
//         {
//             viewAndTransformGroup = contexts.game
//                 .GetGroup(ServerGameMatcher.AllOf(ServerGameMatcher.Transform, GameMatcher.View));
//         }
//         
//         public void Execute()
//         {
//             // log.Debug("Кол-во объектов для обновления позиции "+viewAndTransformGroup.count);
//             foreach (var entity in viewAndTransformGroup)
//             {
//                 Vector3 vector3 = entity.Transform2D.position;
//                 float angle = entity.Transform2D.angle;
//                 entity.view.gameObject.transform.position = vector3;
//                 entity.view.gameObject.transform.rotation = Quaternion.Euler(0, angle, 0);
//             }
//         }
//     }
// }