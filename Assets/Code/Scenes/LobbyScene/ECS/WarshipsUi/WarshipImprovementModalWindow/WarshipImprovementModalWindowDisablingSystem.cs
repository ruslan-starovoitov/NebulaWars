using System.Collections.Generic;
using Code.Scenes.LobbyScene.ECS.CommonLayoutSwitcher;
using Code.Scenes.LobbyScene.Scripts.WarshipsUi;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipImprovementModalWindow
{
    public class WarshipImprovementModalWindowDisablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly WarshipsUiStorage warshipsUiStorage;
        private readonly LobbyLayoutSwitcher lobbyLayoutSwitcher;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipImprovementModalWindowDisablingSystem));
        
        public WarshipImprovementModalWindowDisablingSystem(IContext<LobbyUiEntity> context, 
            WarshipsUiStorage warshipsUiStorage, LobbyLayoutSwitcher lobbyLayoutSwitcher) 
            : base(context)
        {
            this.warshipsUiStorage = warshipsUiStorage;
            this.lobbyLayoutSwitcher = lobbyLayoutSwitcher;
        }
        
        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.DisableWarshipImprovementModalWindow);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.messageDisableWarshipImprovementModalWindow;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            HideModalWindow();
            lobbyLayoutSwitcher.SetCurrentLayer(ShittyUiLayerState.WarshipOverview);
        }
        
        private void HideModalWindow()
        {
            //todo скрыть окно с характеристиками
            warshipsUiStorage.popupWindow.SetActive(false);
        }
    }
}