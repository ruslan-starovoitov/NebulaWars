using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Правильно поворачивает health bar
    /// </summary>
    public class HealthBarRotatingSystem : IExecuteSystem
    {
        private readonly Quaternion rotation;
        private readonly IGroup<ServerGameEntity> healthBars;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarRotatingSystem));

        public HealthBarRotatingSystem(Contexts contexts, Vector3 cameraShift)
        {
            rotation = Quaternion.LookRotation(-cameraShift);
            healthBars = contexts.serverGame.GetGroup(ServerGameMatcher.AllOf(ServerGameMatcher.HealthBar,
                ServerGameMatcher.View));
        }

        public void Execute()
        {
            foreach (var entity in healthBars)
            {
                if (!entity.hasView)
                {
                    log.Error("Если у сущности есть health bar, то обязательно должен быть view");
                    continue;
                }
                entity.view.gameObject.transform.rotation = rotation;
            }
        }
    }
}