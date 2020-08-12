// using Code.Common;
// using Code.Common.Storages;
// using Entitas;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class UpdateDirectionToCenterSystem : IExecuteSystem
//     {
//         private readonly GameContext gameContext;
//         private readonly Image toCenterImage;
//         private const float minimumSqrDist = 1f;
//         private const float fadingSqrDist = 100f;
//         private const float arrowDist = 130f;
//
//         public UpdateDirectionToCenterSystem(Contexts contexts, Image arrowToCenter)
//         {
//             gameContext = contexts.game;
//             toCenterImage = arrowToCenter;
//         }
//
//         public void Execute()
//         {
//             var playerEntity = gameContext.GetEntityWithId(PlayerIdStorage.PlayerEntityId);
//             if (playerEntity != null && gameContext.hasZoneInfo)
//             {
//                 var direction = gameContext.zoneInfo.position - playerEntity.transform.position;
//                 var sqrDist = direction.sqrMagnitude;
//
//                 if (sqrDist <= minimumSqrDist)
//                 {
//                     toCenterImage.color = Color.clear;
//                 }
//                 else
//                 {
//                     var angle = Mathf.Atan2(direction.y, direction.x);
//
//                     var sin = Mathf.Sin(angle);
//                     var cos = Mathf.Cos(angle);
//                     
//                     //-90, потому что стрелка смотрит вверх, а не вправо
//                     toCenterImage.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90f);
//                     toCenterImage.transform.localPosition = new Vector3(arrowDist * cos, arrowDist * sin, 0f);
//
//                     if (sqrDist < fadingSqrDist)
//                     {
//                         var alpha = (sqrDist - minimumSqrDist) / (fadingSqrDist - minimumSqrDist);
//                         toCenterImage.color = new Color(1f, 1f, 1f, alpha);
//                     }
//                 }
//             }
//         }
//     }
// }