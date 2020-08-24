// using Code.Scenes.BattleScene.ECS.Components.Game.TimerComponents;
// using Code.Scenes.BattleScene.ScriptableObjects;
// using Plugins.submodules.SharedCode.Logger;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class DelayedSpawnSystem : BaseTimerSubtractionSystem<DelayedSpawnComponent>
//     {
//         private readonly GameContext gameContext;
//         protected override int PredictedCapacity { get; } = 64;
//         private readonly ILog log = LogManager.CreateLogger(typeof(DelayedSpawnSystem));
//         
//
//         public DelayedSpawnSystem(Contexts contexts) : base(contexts)
//         {
//             gameContext = contexts.game;
//         }
//
//         protected override void OnTimeExpired(GameEntity entity)
//         {
//             if (entity.hasView)
//             {
//                 log.Warn($"Сущность с id {entity.id.value} имела view, пересоздание отложено на один кадр.");
//                 return;
//             }
//             var spawn = entity.delayedSpawn;
//             var obj = ViewObjectsBase.Instance.GetViewObject(spawn.typeId);
//             if(obj.HasSpawnSound && entity.hasSpawnSound) entity.RemoveSpawnSound();
//             obj.FillEntity(gameContext, entity);
//             entity.ReplacePosition(new Vector3(spawn.positionX, spawn.positionY, -0.00001f * entity.id.value));
//             entity.ReplaceDirection(spawn.direction);
//             base.OnTimeExpired(entity);
//         }
//     }
// }