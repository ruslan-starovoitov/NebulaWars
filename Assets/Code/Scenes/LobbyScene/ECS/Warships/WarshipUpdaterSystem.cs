using System.Collections.Generic;
using System.Linq;
using Entitas;
using Entitas.Unity;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Scenes.LobbyScene.ECS.Warships
{
    /// <summary>
    /// Создаёт корабли в лобби при инициализации лобби и при смене скина.
    /// </summary>
    public class WarshipUpdaterSystem:ReactiveSystem<LobbyUiEntity>
    {
        private bool isWarshipCreationCompleted;
        private readonly Transform gameViewsParent;
        private readonly IGroup<LobbyUiEntity> warshipsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipUpdaterSystem));

        public WarshipUpdaterSystem(Contexts contexts, Transform gameViewsParent) 
            : base(contexts.lobbyUi)
        {
            isWarshipCreationCompleted = false;
            this.gameViewsParent = gameViewsParent;
            warshipsGroup = contexts.lobbyUi.GetGroup(LobbyUiMatcher
                .AllOf(LobbyUiMatcher.Warship, LobbyUiMatcher.View));
        }
        
        public bool IsWarshipCreationCompleted()
        {
            return isWarshipCreationCompleted;
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.Warship);
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasWarship;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            foreach (var warshipEntity in entities)
            {
                WarshipDto warshipDto = warshipEntity.warship.warshipDto;
                
                string skinName = warshipDto.GetCurrentSkinName();
                GameObject prefab = Resources.Load<GameObject>("Prefabs/" + skinName);
                GameObject warshipGo = Object.Instantiate(prefab, gameViewsParent, false);
                warshipGo.transform.localScale = new Vector3(1.4f,1.4f,1.4f);
                
                LobbyUiEntity oldWarshipEntity = warshipsGroup.AsEnumerable()
                    .SingleOrDefault(entity =>
                    {
                        return entity.warship.warshipDto.WarshipTypeEnum == warshipDto.WarshipTypeEnum;
                    });
                
                if (oldWarshipEntity != null)
                {
                    oldWarshipEntity.view.gameObject.Unlink();
                    Object.Destroy(oldWarshipEntity.view.gameObject);
                    oldWarshipEntity.Destroy();
                }
                
                warshipEntity.AddView(warshipGo);
                warshipGo.Link(warshipEntity);
            }
            
        }
    }
}