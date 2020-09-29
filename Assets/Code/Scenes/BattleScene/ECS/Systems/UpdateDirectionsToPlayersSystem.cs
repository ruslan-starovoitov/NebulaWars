// using System.Collections.Generic;
// using Code.Scenes.BattleScene.Experimental;
// using Entitas;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Code.BattleScene.ECS.Systems
// {
//     public class UpdateDirectionsToPlayersSystem : IExecuteSystem
//     {
//         private readonly GameContext gameContext;
//         private readonly IGroup<GameEntity> playersGroup;
//         private readonly TeamsColorManager colorManager;
//         private readonly Dictionary<int, Image> arrows;
//         private readonly Camera arrowsCamera;
//         private readonly Canvas arrowsCanvas;
//         private readonly Sprite arrowSprite;
//         private readonly float firstDiagonalRadAngle;
//         private readonly float secondDiagonalRadAngle;
//         private readonly float fourthDiagonalRadAngle;
//         private readonly float halfCanvasWidth;
//         private readonly float halfCanvasHeight;
//         private const float visibleArea = 15f;
//         private TeamsColorManager.ColorsMode colorsMode;
//         private Color[] teamsColors;
//
//         public UpdateDirectionsToPlayersSystem(Contexts contexts, Camera camera, Canvas canvas, Sprite arrow, int playersCount)
//         {
//             colorManager = TeamsColorManager.Instance();
//             colorsMode = colorManager.TeamsColorsMode;
//             gameContext = contexts.game;
//             playersGroup =
//                 gameContext.GetGroup(GameMatcher.AllOf(GameMatcher.Player, GameMatcher.Team, GameMatcher.Transform).NoneOf(GameMatcher.CurrentPlayer, GameMatcher.Hidden));
//             arrowsCamera = camera;
//             arrowsCanvas = canvas;
//             var canvasRect = canvas.GetComponent<RectTransform>().rect;
//             firstDiagonalRadAngle = Mathf.Atan(canvasRect.height / canvasRect.width);
//             secondDiagonalRadAngle = Mathf.PI - firstDiagonalRadAngle;
//             fourthDiagonalRadAngle = Mathf.PI + secondDiagonalRadAngle;
//             halfCanvasWidth = canvasRect.width * 0.5f;
//             halfCanvasHeight = canvasRect.height * 0.5f;
//             arrowSprite = arrow;
//             arrows = new Dictionary<int, Image>(playersCount - 1);
//             teamsColors = colorManager.GetColors(playersCount + 1);
//         }
//
//         public void Execute()
//         {
//             foreach (var arrow in arrows.Values) arrow.gameObject.SetActive(false);
//
//             if (colorsMode != colorManager.TeamsColorsMode)
//             {
//                 colorsMode = colorManager.TeamsColorsMode;
//                 var length = teamsColors.Length;
//                 if (colorsMode == TeamsColorManager.ColorsMode.None)
//                 {
//                     teamsColors = new Color[length];
//                     for (var i = 0; i < length; i++) teamsColors[i] = Color.red;
//                 }
//                 else
//                 {
//                     teamsColors = colorManager.GetColors(length);
//                 }
//             }
//
//             var currentPosition = (Vector2)arrowsCamera.transform.position;
//
//             foreach (var enemyPlayer in playersGroup)
//             {
//                 var accountId = enemyPlayer.player.accountId;
//
//                 if (!arrows.TryGetValue(accountId, out var arrow))
//                 {
//                     var arrowGo = new GameObject("Arrow To " + enemyPlayer.player.nickname);
//
//                     var arrowTrans = arrowGo.transform;
//                     arrowTrans.SetParent(arrowsCanvas.transform);
//                     arrowTrans.SetAsFirstSibling();
//                     arrowTrans.localScale = Vector3.one;
//
//                     arrow = arrowGo.AddComponent<Image>();
//                     arrow.sprite = arrowSprite;
//                     arrow.rectTransform.sizeDelta = arrowSprite.rect.size;
//
//                     arrows.Add(accountId, arrow);
//                 }
//
//                 arrow.gameObject.SetActive(true);
//
//                 var teamId = enemyPlayer.team.id;
//
//                 var direction = enemyPlayer.transform.position - currentPosition;
//
//                 Vector2 arrowLocalPosition;
//
//                 var angle = Mathf.Atan2(direction.y, direction.x);
//                 var angleIsNegative = angle < 0f;
//                 var tempAngle = angleIsNegative ? angle + Mathf.PI : angle;
//
//                 if (tempAngle <= firstDiagonalRadAngle || tempAngle >= secondDiagonalRadAngle)
//                 {
//                     var x = halfCanvasWidth;
//                     var y = halfCanvasWidth * direction.y / direction.x;
//                     arrowLocalPosition = new Vector2(x, y);
//                 }
//                 else
//                 {
//                     var x = halfCanvasHeight * direction.x / direction.y;
//                     var y = halfCanvasHeight;
//                     arrowLocalPosition = new Vector2(x, y);
//                 }
//
//                 var positiveAngle = angleIsNegative ? angle + 2f * Mathf.PI : angle;
//                 var isDownTriangle = positiveAngle > secondDiagonalRadAngle && positiveAngle < fourthDiagonalRadAngle;
//                 if (isDownTriangle) arrowLocalPosition = -arrowLocalPosition;
//
//                 arrowLocalPosition -= arrowLocalPosition.normalized * arrowSprite.rect.height * 0.5f;
//
//                 arrow.transform.localPosition = arrowLocalPosition;
//                 arrow.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * angle - 90f);
//
//                 var color = teamsColors[teamId];
//                 var worldVector = (Vector2)arrowsCamera.ScreenToWorldPoint(arrow.rectTransform.position) - currentPosition;
//
//                 var worldMgn = worldVector.magnitude;
//                 var directionMgn = direction.magnitude;
//
//                 if (worldMgn > directionMgn)
//                 {
//                     arrow.gameObject.SetActive(false);
//                 }
//                 else
//                 {
//                     var percentage = 1f - 0.5f * (directionMgn - worldMgn) / (visibleArea - worldMgn);
//                     if (percentage > 0f)
//                     {
//                         color.a = percentage;
//                         arrow.color = color;
//                     }
//                     else
//                     {
//                         arrow.gameObject.SetActive(false);
//                     }
//                 }
//             }
//         }
//     }
// }