// using System.Collections.Generic;
// using System.Linq;
// using Code.Scenes.BattleScene.Udp.MessageProcessing;
// using Entitas;
// using Plugins.submodules.SharedCode.Logger;
// using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Health;
//
// namespace Code.Scenes.BattleScene.ECS.NewSystems
// {
//     public class MaxHealthUpdaterSystem : IExecuteSystem, IMaxHealthPointsMessagePackStorage
//     {
//         private bool needUpdate;
//         private readonly GameContext gameContext;
//         private readonly ILog log = LogManager.CreateLogger(typeof(HealthUpdaterSystem));
//         private readonly List<MaxHealthPointsMessagePack> messages = new List<MaxHealthPointsMessagePack>();
//         
//         public MaxHealthUpdaterSystem(Contexts contexts)
//         {
//             gameContext = contexts.game;
//         }
//
//         public void SetNewMaxHealthPoints(MaxHealthPointsMessagePack message)
//         {
//             // log.Debug("Кол-во объектов с хп "+message.entityIdToValue.Count);
//             messages.Add(message);
//             needUpdate = true;
//         }
//         
//         public void Execute()
//         {
//             if (!needUpdate)
//             {
//                 return;
//             }
//
//             needUpdate = false;
//             var actualValues = messages.Last();
//             foreach (var pair in actualValues.entityIdToValue)
//             {
//                 ushort entityId = pair.Key;
//                 float health = pair.Value;
//                 var entity = gameContext.GetEntityWithId(entityId);
//                 if (entity == null)
//                 {
//                     log.Debug("Нет сущности с id "+entityId);
//                     continue;
//                 }
//
//                 if (!entity.hasMaxHealth)
//                 {
//                     entity.AddMaxHealth((int)health);
//                 }
//                 else if (entity.maxHealth.value != (int) health)
//                 {
//                     entity.ReplaceHealth((int) health);
//                 }
//                 // log.Debug("Обновление хп "+health);
//             }
//         }
//
//        
//     }
// }