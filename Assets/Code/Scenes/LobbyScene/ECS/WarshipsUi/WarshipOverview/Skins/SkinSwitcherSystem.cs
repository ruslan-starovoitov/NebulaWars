using System.Collections.Generic;
using System.Linq;
using Code.Common;
using Code.Scenes.LobbyScene.Scripts.WarshipsUi;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipOverview.Skins
{
    /// <summary>
    /// Отвечает за показ скинов во время обзора корабля
    /// </summary>
    public class SkinSwitcherSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly LobbyUiContext lobbyUiContext;
        private readonly WarshipsUiStorage warshipsUiStorage;
        private readonly IGroup<LobbyUiEntity> warshipsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(SkinSwitcherSystem));

        
        public SkinSwitcherSystem(Contexts contexts, WarshipsUiStorage warshipsUiStorage)
            : base(contexts.lobbyUi)
        {
            this.warshipsUiStorage = warshipsUiStorage;
            lobbyUiContext = contexts.lobbyUi;
            warshipsGroup = lobbyUiContext.GetGroup(LobbyUiMatcher.Warship);
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.WarshipOverviewCurrentSkinModel);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasWarshipOverviewCurrentSkinModel;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            var skinModel = entities.Last().warshipOverviewCurrentSkinModel;
            int newSkinIndex = skinModel.skinIndex;
            string skinName = skinModel.warshipDto.Skins[newSkinIndex].Name;
            DestroyCurrentSkin();
            SpawnSkin(skinName);
            skinModel.warshipDto.CurrentSkinIndex = newSkinIndex;
        }

        private void DestroyCurrentSkin()
        {
            warshipsUiStorage.warshipRoot.transform.DestroyAllChildren();
        }
        
        private void SpawnSkin(string skinName)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + skinName);
            Transform parent = warshipsUiStorage.warshipRoot.transform;
            GameObject warship = Object.Instantiate(prefab, parent, false);
            warship.transform.position = new Vector3(0,0,55);
            warship.transform.localScale = new Vector3(1.4f,1.4f,1.4f);
        }
    }
}