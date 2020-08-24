// using System.Collections.Generic;
// using Code.Common;
// using Code.Common.Storages;
// using Code.Scenes.BattleScene.Experimental;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems.AudioSystems
// {
//     public class DeathSoundSystem : ReactiveSystem<GameEntity>
//     {
//         private readonly GameContext gameContext;
//         private readonly SoundManager _soundManager = SoundManager.Instance();
//
//         public DeathSoundSystem(Contexts contexts) : base(contexts.game)
//         {
//             gameContext = contexts.game;
//         }
//
//         protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
//         {
//             return context.CreateCollector(GameMatcher.Destroyed.Added());
//         }
//
//         protected override bool Filter(GameEntity entity)
//         {
//             return entity.hasView && entity.hasDeathSound && entity.isDestroyed && entity.hasTransform && !entity.isHidden && !entity.hasDelayedSpawn;
//         }
//
//         protected override void Execute(List<GameEntity> entities)
//         {
//             var playerEntity = gameContext.GetEntityWithId(PlayerIdStorage.PlayerEntityId);
//             if (playerEntity == null) return;
//
//             foreach (var e in entities)
//             {
//                 var go = e.view.gameObject;
//
//                 var source = go.GetComponent<AudioSource>();
//                 if (source == null) source = go.AddComponent<AudioSource>();
//
//                 var dist = (playerEntity.transform.position - e.transform.position).magnitude;
//
//                 if (dist <= SoundManager.MaxBattleSoundDistance)
//                 {
//                     var clip = e.deathSound.value;
//                     _soundManager.PlayGameSound(source, clip);
//
//                     if (e.hasDestroyTimer)
//                     {
//                         if(e.destroyTimer.time < clip.length) e.ReplaceDestroyTimer(clip.length);
//                     }
//                     else
//                     {
//                         e.AddDestroyTimer(clip.length);
//                     }
//                 }
//             }
//         }
//     }
// }