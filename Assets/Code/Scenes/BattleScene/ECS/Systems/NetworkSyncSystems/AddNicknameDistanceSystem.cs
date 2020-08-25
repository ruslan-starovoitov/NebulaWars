// using System.Collections.Generic;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
// {
//     public class AddNicknameDistanceSystem : ReactiveSystem<ServerGameEntity>
//     {
//         public AddNicknameDistanceSystem(Contexts contexts) : base(contexts.game)
//         { }
//
//         protected override ICollector<ServerGameEntity> GetTrigger(IContext<ServerGameEntity> context)
//         {
//             var matcher = GameMatcher.AllOf(ServerGameMatcher.View, GameMatcher.Player).NoneOf(ServerGameMatcher.NicknameDistance);
//             return context.CreateCollector(matcher);
//         }
//
//         protected override bool Filter(ServerGameEntity entity)
//         {
//             return entity.hasView && entity.hasPlayer && !entity.hasNicknameDistance;
//         }
//
//         protected override void Execute(List<ServerGameEntity> entities)
//         {
//             foreach (ServerGameEntity e in entities)
//             {
//                 var go = e.view.gameObject;
//                 var sprite = go.GetComponent<SpriteRenderer>();
//                 e.AddNicknameDistance(sprite.bounds.max.magnitude);
//             }
//         }
//     }
// }