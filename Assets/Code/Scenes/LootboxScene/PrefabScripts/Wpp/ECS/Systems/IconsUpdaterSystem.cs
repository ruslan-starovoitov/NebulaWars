using Code.Scenes.LobbyScene.ECS.Extensions;
    using Entitas;
    using Plugins.submodules.SharedCode.Logger;
    using UnityEngine;

    namespace Code.Scenes.LootboxScene.PrefabScripts.Wpp.ECS.Systems
    {
        /// <summary>
        /// Меняет позиции GO движущихся наград.
        /// </summary>
        public class IconsUpdaterSystem:IExecuteSystem
        {
            private readonly RectTransform upperObject;
            private readonly IGroup<WppAccrualEntity> movingAwardsGroup;
            private readonly ILog log = LogManager.CreateLogger(typeof(IconsUpdaterSystem));

            public IconsUpdaterSystem(Contexts contexts, RectTransform upperObject)
            {
                this.upperObject = upperObject;
                var context = contexts.wppAccrual;
                movingAwardsGroup = context.GetGroup(WppAccrualMatcher
                    .AllOf(WppAccrualMatcher.MovingIcon, WppAccrualMatcher.View, WppAccrualMatcher.Position));
            }
            
            public void Execute()
            {
                foreach (var entity in movingAwardsGroup)
                {
                    entity.view.gameObject.transform.position = entity.position.value;
                    entity.view.gameObject.transform.localScale = entity.scale.scale;
                    if (entity.image.image != null)
                    {
                        var tmpColor =  entity.image.image.color;
                        tmpColor.a = entity.alpha.alpha;
                        entity.image.image.color = tmpColor;

                    }

                    if (entity.movingIcon.IsRaiseUpNeeded())
                    {
                        entity.view.gameObject.transform.SetParent(upperObject, false);
                        entity.movingIcon.TurnOffRaiseUp();
                    }
                }
            }
        }
    }