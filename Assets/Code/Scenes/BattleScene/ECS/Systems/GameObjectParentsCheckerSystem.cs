// using System;
// using Entitas;
// using Plugins.submodules.SharedCode.Logger;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class GameObjectParentsCheckerSystem : IExecuteSystem
//     {
//         private readonly GameContext gameContext;
//         private readonly IGroup<ServerGameEntity> withParents;
//         private readonly ILog log = LogManager.CreateLogger(typeof(GameObjectParentsCheckerSystem));
//         
//         public GameObjectParentsCheckerSystem(Contexts contexts)
//         {
//             gameContext = contexts.serverGame;
//             withParents = gameContext.GetGroup(GameMatcher
//                 .AllOf(ServerGameMatcher.View, GameMatcher.Parent).NoneOf(ServerGameMatcher.Destroyed));
//         }
//
//         public void Execute()
//         {
//             try
//             {
//                 foreach (GameEntity e in withParents)
//                 {
//                     GameEntity parentEntity = gameContext.GetEntityWithId(e.parent.id);
//
//                     if (parentEntity != null && parentEntity.hasView && !parentEntity.isDestroyed)
//                     {
//                         e.view.gameObject.transform.SetParent(parentEntity.view.gameObject.transform);
//                     }
//                 }
//             }
//             catch (Exception e)
//             {
//                 log.Error(e.Message+" "+e.StackTrace);   
//             }
//         }
//     }
// }