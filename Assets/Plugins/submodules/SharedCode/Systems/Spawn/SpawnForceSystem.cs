using Entitas;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Spawn
{
    /// <summary>
    /// Добавляет Force
    /// Удаляет force компонент
    /// </summary>
    public class SpawnForceSystem : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<ServerGameEntity> needSpawnForceGroup;

        public SpawnForceSystem(Contexts contexts)
        {
            needSpawnForceGroup = contexts.serverGame
                .GetGroup(ServerGameMatcher.AllOf(ServerGameMatcher.Rigidbody, ServerGameMatcher.SpawnForce));
        }
        
        public void Execute()
        {
            foreach (var entity in needSpawnForceGroup)
            {
                Rigidbody rigidbody = entity.rigidbody.value;
                var forceVector = entity.spawnForce.vector3;
                rigidbody.AddForce(forceVector, ForceMode.VelocityChange);
            }
        }

        public void Cleanup()
        {
            var entities = needSpawnForceGroup.GetEntities();
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                entity.RemoveSpawnForce();
            }
        }
    }
}