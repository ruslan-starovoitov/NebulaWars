// using System.Collections.Generic;
// using Code.Common;
// using Code.Common.Storages;
// using Code.Scenes.BattleScene.Experimental;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.AudioSystems
// {
//     public class SpawnSoundSystem : ReactiveSystem<ServerGameEntity>
//     {
//         private readonly GameContext gameContext;
//         private readonly SoundManager _soundManager = SoundManager.Instance();
//
//         public SpawnSoundSystem(Contexts contexts) : base(contexts.game)
//         {
//             gameContext = contexts.serverGame;
//         }
//
//         protected override ICollector<ServerGameEntity> GetTrigger(IContext<ServerGameEntity> context)
//         {
//             var matcher = GameMatcher.AllOf(ServerGameMatcher.View, GameMatcher.SpawnSound);
//             return context.CreateCollector(matcher);
//         }
//
//         protected override bool Filter(ServerGameEntity entity)
//         {
//             return entity.hasView && entity.hasSpawnSound && entity.hasTransform && !entity.isHidden;
//         }
//
//         protected override void Execute(List<ServerGameEntity> entities)
//         {
//             var playerEntity = gameContext.GetEntityWithId(PlayerIdStorage.PlayerEntityId);
//             if(playerEntity == null) return;
//
//             foreach (var e in entities)
//             {
//                 var go = e.view.gameObject;
//
//                 var source = go.GetComponent<AudioSource>();
//                 if (source == null) source = go.AddComponent<AudioSource>();
//
//                 var dist = (playerEntity.Transform2D.position - e.Transform2D.position).magnitude;
//
//                 if (dist <= SoundManager.MaxBattleSoundDistance)
//                 {
//                     _soundManager.PlayGameSound(source, e.spawnSound.value);
//                 }
//             }
//         }
//     }
// }