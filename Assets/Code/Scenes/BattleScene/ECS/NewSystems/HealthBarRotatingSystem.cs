using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Правильно поворачивает health bar после спавна
    /// </summary>
    public class HealthBarRotatingSystem : IExecuteSystem
    {
        private readonly Quaternion rotation;
        private readonly IGroup<GameEntity> healthBars;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthBarRotatingSystem));

        public HealthBarRotatingSystem(Contexts contexts, Vector3 cameraShift)
        {
            rotation = Quaternion.LookRotation(-cameraShift);
            healthBars = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.HealthBar, GameMatcher.View));
        }

        public void Execute()
        {
            foreach (var entity in healthBars)
            {
                if (!entity.hasView)
                {
                    log.Error("Если у сущности есть health bar, то обязаьельно должен быть view");
                    continue;
                }
                entity.view.gameObject.transform.rotation = rotation;
            }
        }
    }
}