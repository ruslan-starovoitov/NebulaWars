﻿using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class HealthBarPositionUpdaterSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> healthBarsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarPositionUpdaterSystem));

        public HealthBarPositionUpdaterSystem(Contexts contexts)
        {
            healthBarsGroup = contexts.game.GetGroup(GameMatcher.HealthBar);
        }

        public void Execute()
        {
            foreach (var entity in healthBarsGroup)
            {
                if (!entity.hasViewTransform)
                {
                    log.Error("У каждой полоски жизни должен быть hasViewTransform");
                    continue;
                }
                GameEntity parent = entity.healthBar.parent;
                if (!parent.hasViewTransform)
                {
                    log.Error("У родителя полоски жизни должен быть viewTransfrom");
                    continue;
                }
                
                Vector3 tmpPos = entity.viewTransform.value.position;
                Vector3 parentPos = parent.viewTransform.value.position;
                Vector3 actualPosition = new Vector3(parentPos.x, tmpPos.y, parentPos.z);
                entity.viewTransform.value.position = actualPosition;
            }
        }
    }
}