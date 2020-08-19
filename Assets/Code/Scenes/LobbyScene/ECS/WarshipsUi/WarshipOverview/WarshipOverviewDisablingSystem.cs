using System.Collections.Generic;
using Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher;
using Code.Scenes.LobbyScene.Scripts.WarshipsUi;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipOverview
{
    public class WarshipOverviewDisablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly WarshipsUiStorage warshipsUiStorage;
        private readonly LobbyLayoutSwitcher lobbyLayoutSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipOverviewDisablingSystem));
        
        public WarshipOverviewDisablingSystem(IContext<LobbyUiEntity> context, WarshipsUiStorage warshipsUiStorage,
            LobbyLayoutSwitcher lobbyLayoutSwitcher) 
            : base(context)
        {
            this.warshipsUiStorage = warshipsUiStorage;
            this.lobbyLayoutSwitcher = lobbyLayoutSwitcher;
        }
        
        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.DisableWarshipOverviewUiLayer);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.messageDisableWarshipOverviewUiLayer;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            
            Hide();
            lobbyLayoutSwitcher.SetCurrentLayer(ShittyUiLayerState.WarshipsList);
        }
        
        private void Hide()
        {
            warshipsUiStorage.hint.SetActive(true);
            warshipsUiStorage.warshipListRootGameObject.SetActive(true);
            warshipsUiStorage.warshipRootGameObject.SetActive(false);
            warshipsUiStorage.warshipRoot.SetActive(false);
        }
    }
}