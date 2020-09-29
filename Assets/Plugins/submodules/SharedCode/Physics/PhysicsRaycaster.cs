using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.submodules.SharedCode.Physics
{
    public class PhysicsRaycaster
    {
        private readonly Scene scene;
        private readonly PhysicsScene physicsScene;
        private readonly ILog log = LogManager.CreateLogger(typeof(PhysicsRaycaster));
        private readonly int ignoreColliders = ~LayerMask.GetMask("Projectiles");

        public PhysicsRaycaster(Scene scene)
        {
            this.scene = scene;
            physicsScene = scene.GetPhysicsScene();
        }
        
        public bool Raycast(Vector3 origin, Vector3 direction, float maxDistance, out RaycastHit raycastHit)
        {
            return physicsScene.Raycast(origin, direction, out raycastHit, maxDistance, ignoreColliders);
        }
    }
}