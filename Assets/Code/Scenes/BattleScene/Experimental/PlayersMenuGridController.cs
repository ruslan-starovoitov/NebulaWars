// using System.Collections.Generic;
// using Code.Common.Storages;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Code.Scenes.BattleScene.Experimental
// {
//     [RequireComponent(typeof(GridLayoutGroup))]
//     public class PlayersMenuGridController : MonoBehaviour
//     {
//         [SerializeField] private PlayerInfoObject prototype;
//         [SerializeField] private bool isLeft;
//         private Dictionary<int, PlayerInfoObject> accountIdsToInfos;
//         private Dictionary<int, PlayerInfoObject> teamsToInfos;
//         private int startIndex;
//         private int endIndex;
//         private int capacity;
//         private int totalCount;
//
//         public void Fill()
//         {
//             var matchModel = MyMatchDataStorage.Instance.GetMatchModel();
//             var models = matchModel.PlayerModels;
//             totalCount = models.Length;
//             if (isLeft)
//             {
//                 startIndex = 0;
//                 endIndex = totalCount / 2;
//             }
//             else
//             {
//                 startIndex = totalCount / 2;
//                 endIndex = totalCount;
//             }
//
//             capacity = endIndex - startIndex;
//             accountIdsToInfos = new Dictionary<int, PlayerInfoObject>(capacity);
//             teamsToInfos = new Dictionary<int, PlayerInfoObject>(capacity);
//
//             for (var i = startIndex; i < endIndex; i++)
//             {
//                 var model = models[i];
//                 var info = Instantiate(prototype, transform);
//                 info.SetInfo(model);
//                 accountIdsToInfos.Add(model.AccountId, info);
//                 teamsToInfos.Add(i + 1, info);
//             }
//         }
//
//         private void Awake()
//         {
//             var grid = GetComponent<GridLayoutGroup>();
//             var totalHeight = GetComponent<RectTransform>().rect.height;
//             var cellHeight = grid.cellSize.y;
//             var spacesPlace = totalHeight - cellHeight * capacity;
//             grid.spacing = new Vector2(grid.spacing.x, spacesPlace / (capacity - 1));
//
//             UpdateColors();
//         }
//
//         public void CheckKill(int accountId)
//         {
//             if (accountIdsToInfos.TryGetValue(accountId, out var info)) info.MarkAsKilled();
//         }
//
//         public void UpdateColors()
//         {
//             var colors = TeamsColorManager.Instance().GetColors(totalCount + 1);
//             foreach (var pair in teamsToInfos) pair.Value.SetColor(colors[pair.Key]);
//         }
//     }
// }