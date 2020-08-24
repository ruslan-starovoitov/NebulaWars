// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class ManyDelayedRecreationsSystem : IExecuteSystem
//     {
//         private readonly IGroup<GameEntity> delayedRecreationsGroup;
//
//         public ManyDelayedRecreationsSystem(Contexts contexts)
//         {
//             delayedRecreationsGroup = contexts.game.GetGroup(GameMatcher.ManyDelayedRecreations);
//         }
//
//         public void Execute()
//         {
//             var deltaTime = Time.deltaTime;
//
//             foreach (var e in delayedRecreationsGroup)
//             {
//                 var queue = e.manyDelayedRecreations.components;
//                 foreach (var recreationComponent in queue)
//                 {
//                     recreationComponent.time -= deltaTime;
//                 }
//             }
//         }
//     }
// }