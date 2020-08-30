using System.Collections.Generic;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using Plugins.submodules.SharedCode.Systems.Spawn;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scenes.BattleScene.ECS
{
    /// <summary>
    /// В отдельной физической сцене показывает позиции всех объектов согласно последнему пришедшему состоянию.
    /// </summary>
    public class ServerGameStateDebugSystem : IExecuteSystem, IInitializeSystem
    {
        private int lastShowedTickNumber;
        private PhysicsScene physicsScene;
        private readonly PrefabsStorage prefabsStorage;
        private readonly ServerGameStateBuffer serverGameStateBuffer;
        private readonly VectorValidator vectorValidator = new VectorValidator();
        private readonly ILog log = LogManager.CreateLogger(typeof(ServerGameStateDebugSystem));
        private readonly Dictionary<ushort, GameObject> dictionary = new Dictionary<ushort, GameObject>();
        
        public ServerGameStateDebugSystem(ServerGameStateBuffer serverGameStateBuffer, PrefabsStorage prefabsStorage)
        {
            this.serverGameStateBuffer = serverGameStateBuffer;
            this.prefabsStorage = prefabsStorage;
        }
        
        public void Initialize()
        {
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
            Scene matchScene = SceneManager.LoadScene("EmptyScene", loadSceneParameters);
            physicsScene = matchScene.GetPhysicsScene();
        }
        
        public void Execute()
        {
            int lastSavedTickNumber = serverGameStateBuffer.GetLastSavedTickNumber();
            if (serverGameStateBuffer.GetLastSavedTickNumber() == lastShowedTickNumber)
            {
                return;
            }
            
            lastShowedTickNumber = lastSavedTickNumber;
            var newestGameState = serverGameStateBuffer.GetNewestGameState();
            
            HashSet<ushort> needDelete = new HashSet<ushort>(dictionary.Keys);
            foreach (var pair in newestGameState.transforms)
            {
                ushort entityId = pair.Key;
                var viewTransform = pair.Value;

                Vector3 position = viewTransform.GetPosition();
                Quaternion rotation = Quaternion.AngleAxis(viewTransform.Angle, Vector3.up);

                if (!vectorValidator.TryValidate(position))
                {
                    log.Debug("Проблема позиции");
                    continue;
                }
                
                needDelete.Remove(entityId);
                //Такой обьект уже есть на сцене
                if (dictionary.TryGetValue(entityId, out var gameObject))
                {
                    //Обновить
                    gameObject.transform.position = position;
                    gameObject.transform.rotation = rotation;
                }
                else
                {
                    //Создать
                    GameObject prefab = prefabsStorage.GetPrefab(viewTransform.viewTypeEnum);
                    GameObject go = Object.Instantiate(prefab, position, rotation);
                    dictionary.Add(entityId, go);
                }
            }

            //Удалить лишние объекты
            foreach (var entityId in needDelete)
            {
                Object.Destroy(dictionary[entityId]);
            }
        }
    }
}