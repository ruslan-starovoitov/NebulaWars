// using Entitas;
// using UnityEngine;
//
// // ReSharper disable once CheckNamespace
// namespace Code.BattleScene.ECS.Systems
// {
//     public class ZoneMoveSystem : IExecuteSystem
//     {
//         private readonly GameContext gameContext;
//         private readonly GameObject zoneObject;
//
//         public ZoneMoveSystem(Contexts contexts, GameObject zone)
//         {
//             gameContext = contexts.serverGame;
//             zoneObject = zone;
//         }
//
//         public void Execute()
//         {
//             if(!gameContext.hasZoneInfo) return;
//             zoneObject.transform.position = gameContext.zoneInfo.position;
//             var scale = 2f * gameContext.zoneInfo.radius;
//             zoneObject.transform.localScale = new Vector3(scale, scale, scale);
//         }
//     }
// }