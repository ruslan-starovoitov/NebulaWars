using System;
using JetBrains.Annotations;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Plugins.submodules.SharedCode.Physics
{
    /// <summary>
    /// Создаёт обьект в физической сцене матча.
    /// </summary>
    public class PhysicsSpawner
    {
        private readonly Scene scene;
        private readonly ILog log = LogManager.CreateLogger(typeof(PhysicsSpawner));
        private readonly int ignoreRaycastLayerNumber = LayerMask.NameToLayer("Ignore Raycast");
        
        public PhysicsSpawner(Scene scene)
        {
            this.scene = scene;
        }

        public void Ignore(Collider[] colliders1, Collider[] colliders2)
        {
            //todo это чудо не работает
            // log.Debug($"colliders1.Length = {colliders1.Length}");
            // log.Debug($"colliders2.Length = {colliders2.Length}");
            foreach (var collider1 in colliders1)
            {
                foreach (var collider2 in colliders2)
                {
                    // log.Debug(collider1.name+" "+collider2.name);
                    UnityEngine.Physics.IgnoreCollision(collider1, collider2);
                }
            }
        }
        
        [NotNull]
        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }
            
            GameObject go = Object.Instantiate(prefab, position, rotation);
            if (go == null)
            {
                throw new Exception("Не удалось создать корабль");
            }
            
            SceneManager.MoveGameObjectToScene(go, scene);
            return go;
        }

        public GameObject SpawnProjectile(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject go = Object.Instantiate(prefab, position, rotation);
            go.layer = ignoreRaycastLayerNumber;
            SceneManager.MoveGameObjectToScene(go, scene);
            return go;
        }
    }
}