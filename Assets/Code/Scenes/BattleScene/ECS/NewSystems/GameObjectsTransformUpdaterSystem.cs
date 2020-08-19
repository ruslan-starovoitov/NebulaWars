using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Задаёт всем обьектам позициб и поворот по данным из памяти
    /// </summary>
    public class GameObjectsTransformUpdaterSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> viewAndTransformGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(GameObjectsTransformUpdaterSystem));

        public GameObjectsTransformUpdaterSystem(Contexts contexts)
        {
            viewAndTransformGroup = contexts.game
                .GetGroup(GameMatcher.AllOf(GameMatcher.Transform, GameMatcher.View));
        }
        
        public void Execute()
        {
            // log.Debug("Кол-во объектов для обновления позиции "+viewAndTransformGroup.count);
            foreach (var entity in viewAndTransformGroup)
            {
                Vector3 vector3 = entity.transform.position;
                float angle = entity.transform.angle;
                entity.view.gameObject.transform.position = vector3;
                entity.view.gameObject.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }
    }
}