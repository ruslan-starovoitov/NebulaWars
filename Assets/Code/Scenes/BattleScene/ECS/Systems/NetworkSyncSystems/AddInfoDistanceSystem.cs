// using System.Collections.Generic;
// using Code.Common.Logger;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
// {
//     public class AddInfoDistanceSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly ILog log = LogManager.CreateLogger(typeof(AddInfoDistanceSystem));
//
//         public AddInfoDistanceSystem(Contexts contexts) : base(contexts.game)
//         { }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             return context.CreateCollector(GameMatcher.View);
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.hasView && !entity.hasInfoDistance;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             foreach (GameEntity e in entities)
//             {
//                 var go = e.view.gameObject;
//                 var renderer = go.GetComponent<Renderer>();
//                 if (renderer != null)
//                 {
//                     e.AddInfoDistance(renderer.bounds.max.magnitude);
//                 }
//                 else
//                 {
//                     log.Warn($"Отсутствует рендерер у объекта {(e.hasViewType ? e.viewType.ToString() : "с id " + e.id.value)}.");
//                 }
//             }
//         }
//     }
// }