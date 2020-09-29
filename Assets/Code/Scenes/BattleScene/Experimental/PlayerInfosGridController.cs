// using Code.Common;
// using Code.Common.Storages;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Code.Scenes.BattleScene.Experimental
// {
//     [RequireComponent(typeof(GridLayoutGroup))]
//     public class PlayerInfosGridController : Singleton<PlayerInfosGridController>
//     {
//         [SerializeField] private PlayerInfoObject prototype;
//
//         private void Start()
//         {
//             var matchModel = MyMatchDataStorage.Instance.GetMatchModel();
//             var models = matchModel.PlayerModels;
//             var colors = TeamsColorManager.Instance().GetColors(models.Length + 1);
//             for (var i = 0; i < models.Length; i++)
//             {
//                 var model = models[i];
//                 var info = Instantiate(prototype, transform);
//                 info.SetInfo(model);
//                 info.SetColor(colors[i + 1]);
//             }
//         }
//     }
// }