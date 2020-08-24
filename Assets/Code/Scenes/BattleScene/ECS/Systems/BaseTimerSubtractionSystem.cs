// using System;
// using System.Collections.Generic;
// using Code.Scenes.BattleScene.ECS.Components.Game.TimerComponents;
// using Entitas;
// using UnityEngine;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class BaseTimerSubtractionSystem<TTimerComponent> : IExecuteSystem
//         where TTimerComponent : TimerComponent
//     {
//         private readonly int componentIndex;
//         private readonly IGroup<GameEntity> withTimerGroup;
//         private readonly List<GameEntity> buffer;
//         protected virtual int PredictedCapacity { get; } = 16;
//
//         public BaseTimerSubtractionSystem(Contexts contexts)
//         {
//             componentIndex = Array.IndexOf(GameComponentsLookup.componentTypes, typeof(TTimerComponent));
//
//             var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(componentIndex);
//             matcher.componentNames = GameComponentsLookup.componentNames;
//
//             withTimerGroup = contexts.game.GetGroup(matcher);
//             buffer = new List<GameEntity>(PredictedCapacity);
//         }
//
//         public void Execute()
//         {
//             var deltaTime = Time.deltaTime;
//
//             foreach (var e in withTimerGroup.GetEntities(buffer))
//             {
//                 var component = (TTimerComponent) e.GetComponent(componentIndex);
//                 var newTime = component.time - deltaTime;
//
//                 if (newTime > 0f)
//                 {
//                     component.time = newTime;
//                 }
//                 else
//                 {
//                     OnTimeExpired(e);
//                 }
//             }
//         }
//
//         protected virtual void OnTimeExpired(GameEntity entity)
//         {
//             entity.RemoveComponent(componentIndex);
//         }
//     }
// }