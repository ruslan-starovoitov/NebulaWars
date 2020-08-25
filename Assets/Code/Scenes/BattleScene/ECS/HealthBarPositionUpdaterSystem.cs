using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class HealthBarPositionUpdaterSystem : IExecuteSystem
    {
        private readonly IGroup<ServerGameEntity> healthBarsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarPositionUpdaterSystem));

        public HealthBarPositionUpdaterSystem(Contexts contexts)
        {
            healthBarsGroup = contexts.serverGame.GetGroup(ServerGameMatcher.HealthBar);
        }

        public void Execute()
        {
            foreach (var entity in healthBarsGroup)
            {
                if (!entity.hasTransform)
                {
                    log.Error("У каждой полоски жизни должен быть hasViewTransform");
                    continue;
                }
                var parent = entity.healthBar.parent;
                if (!parent.hasTransform)
                {
                    log.Error("У родителя полоски жизни должен быть viewTransfrom");
                    continue;
                }
                
                Vector3 tmpPos = entity.transform.value.position;
                Vector3 parentPos = parent.transform.value.position;
                Vector3 actualPosition = new Vector3(parentPos.x, tmpPos.y, parentPos.z);
                entity.transform.value.position = actualPosition;
            }
        }
    }
}