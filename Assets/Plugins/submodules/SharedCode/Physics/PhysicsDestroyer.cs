using Entitas.VisualDebugging.Unity;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Physics
{
    public class PhysicsDestroyer
    { 
        public void Destroy(GameObject gameObject)
        {
            gameObject.DestroyGameObject();
        }
    }
}