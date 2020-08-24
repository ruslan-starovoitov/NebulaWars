// using Code.Scenes.BattleScene.ECS.Components.Game.TimerComponents;
// using Code.Scenes.BattleScene.ScriptableObjects;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class DelayedDestroySystem : BaseTimerSubtractionSystem<DelayedDestroyComponent>
//     {
//         protected override int PredictedCapacity { get; } = 64;
//
//         public DelayedDestroySystem(Contexts contexts) : base(contexts)
//         { }
//
//         protected override void OnTimeExpired(ServerGameEntity entity)
//         {
//             base.OnTimeExpired(entity);
//             if (entity.hasSpeed) entity.ReplaceSpeed(Vector2.zero, 0f);
//             entity.isDestroyed = true;
//         }
//     }
// }