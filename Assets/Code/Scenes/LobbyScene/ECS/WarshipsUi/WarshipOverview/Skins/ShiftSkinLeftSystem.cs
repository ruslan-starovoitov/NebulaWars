using Code.Common;
using Entitas;
using System.Collections.Generic;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.WarshipsUi.WarshipOverview.Skins
{
    /// <summary>
    /// Отвечает за листание скинов
    /// </summary>
    public class ShiftSkinLeftSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly LobbyUiContext lobbyUiContext;
        private readonly UiSoundsManager lobbySoundsManager;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShiftSkinLeftSystem));

        public ShiftSkinLeftSystem(Contexts contexts, UiSoundsManager lobbySoundsManager) 
            : base(contexts.lobbyUi)
        {
            lobbyUiContext = contexts.lobbyUi;
            this.lobbySoundsManager = lobbySoundsManager;
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.ShiftSkinLeft);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.messageShiftSkinLeft;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            var model = lobbyUiContext.warshipOverviewCurrentSkinModel;
            int currentSkinIndex = model.skinIndex;
            int skinsCount =  model.warshipDto.Skins.Count;
            if (currentSkinIndex == skinsCount-1)
            {
                log.Warn("Нельзя сдвинуть скины влево. Сейчас показывается крайний правый.");
            }
            else
            {
                lobbyUiContext.ReplaceWarshipOverviewCurrentSkinModel(++model.skinIndex, model.warshipDto);
                lobbySoundsManager.PlayWarshipChangingLeft();
            }
        }
    }
}