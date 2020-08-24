// using Code.Scenes.BattleScene.Udp.MessageProcessing;
// using Code.Scenes.BattleScene.Udp.MessageProcessing.Synchronizers;
// using Entitas;
// using Entitas.Unity;
// using UnityEngine;
// using Object = UnityEngine.Object;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.TearDownSystems
// {
//     public class GameContextClearSystem : ITearDownSystem
//     {
//         private readonly Contexts contexts;
//
//         public GameContextClearSystem(Contexts contexts)
//         {
//             this.contexts = contexts;
//         }
//         
//         public void TearDown()
//         {
//             var entities = contexts.serverGame.GetEntities();
//             foreach (var entity in entities)
//             {
//                 if (entity.hasView)
//                 {
//                     GameObject gameObject = entity.view.gameObject;
//                     if (gameObject != null)
//                     {
//                         gameObject.Unlink();
//                         Object.Destroy(gameObject);   
//                     }
//                 }
//                 entity.Destroy();
//             }
//         }
//     }
// }