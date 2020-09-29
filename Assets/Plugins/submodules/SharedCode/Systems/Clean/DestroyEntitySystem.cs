using Entitas;
using Entitas.Unity;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Physics;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Clean
{
    /// <summary>
    /// В конце кадра уничтожает сущность вместе с view если есть флаг ServerGameMatcher.Destroyed
    /// </summary>
    public class DestroyEntitySystem:ICleanupSystem
    {
        private readonly PhysicsDestroyer physicsDestroyer;
        private readonly IGroup<ServerGameEntity> needDestroyGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(DestroyEntitySystem));

        public DestroyEntitySystem(Contexts contexts, PhysicsDestroyer physicsDestroyer)
        {
            this.physicsDestroyer = physicsDestroyer;
            needDestroyGroup = contexts.serverGame.GetGroup(ServerGameMatcher.Destroyed);
        }

        public void Cleanup()
        {
            var needDestroy = needDestroyGroup.GetEntities();
            for (int i = 0; i < needDestroy.Length; i++)
            {
                ServerGameEntity entity = needDestroy[i];
                if (entity.hasTransform)
                {
                    GameObject go = entity.transform.value.gameObject;
                    go.Unlink();
                    physicsDestroyer.Destroy(go);
                }
                
                entity.Destroy();
            }
        }
    }
}