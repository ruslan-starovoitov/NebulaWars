using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    /// <summary>
    /// Зануляет скорости кораблям. Возможно, это поможет с локальным предсказанием.
    /// </summary>
    public class StopWarshipsSystem : IExecuteSystem
    {
        private readonly IGroup<ServerGameEntity> warshipsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(StopWarshipsSystem));
        
        public StopWarshipsSystem(Contexts contexts)
        {
            var matcher = ServerGameMatcher.AllOf(ServerGameMatcher.Transform, ServerGameMatcher.Player);
            warshipsGroup = contexts.serverGame.GetGroup(matcher);
        }
        
        public void Execute()
        {
            // log.Info("Количество остановленнх кораблей "+warshipsGroup.count);
            foreach (var entity in warshipsGroup)
            {
                entity.rigidbody.value.velocity = Vector3.zero;
                entity.rigidbody.value.angularVelocity = Vector3.zero;
            }
        }
    }
}