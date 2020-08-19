using System.Collections.Generic;
using System.Linq;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.LobbyScene.ECS.Warships
{
    /// <summary>
    /// Включает/выключает объекты кораблей при изменении типа текущего
    /// </summary>
    public class WarshipsEnablingSystem : ReactiveSystem<LobbyUiEntity>
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipsEnablingSystem));
        private readonly IGroup<LobbyUiEntity> warshipsGroup;

        public WarshipsEnablingSystem(Contexts contexts)
            : base(contexts.lobbyUi)
        {
            warshipsGroup = contexts.lobbyUi.GetGroup(LobbyUiMatcher
                .AllOf(LobbyUiMatcher.Warship, LobbyUiMatcher.View));
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.CurrentWarshipTypeEnum);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasCurrentWarshipTypeEnum;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            WarshipTypeEnum warshipTypeEnum = entities.Last().currentWarshipTypeEnum.value;
            foreach (var lobbyUiEntity in warshipsGroup.GetEntities())
            {
                bool enable = warshipTypeEnum == lobbyUiEntity.warship.warshipDto.WarshipTypeEnum;
                lobbyUiEntity.view.gameObject.SetActive(enable);
                // if (enable)
                // {
                //     log.Debug("Включение корабля "+lobbyUiEntity.warship.warshipDto.WarshipTypeEnum.ToString());
                // }
            }
        }
    }
}