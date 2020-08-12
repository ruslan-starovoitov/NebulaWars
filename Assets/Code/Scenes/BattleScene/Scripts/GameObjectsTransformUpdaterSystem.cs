using Code.Common.Logger;
using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
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
                float x = entity.transform.position.x;
                float y = entity.transform.position.y;
                float z = entity.transform.position.z;
                Vector3 vector3 = new Vector3(x, y, z);
                entity.view.gameObject.transform.position = vector3;
                entity.view.gameObject.transform.rotation = Quaternion.Euler(0,entity.transform.angle, 0);
            }
        }
    }
}