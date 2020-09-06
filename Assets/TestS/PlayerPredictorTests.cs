using System;
using System.Collections;
using System.Text.RegularExpressions;
using Code.Scenes.BattleScene.ECS;
using Code.Scenes.BattleScene.Experimental.Prediction;
using NUnit.Framework;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using Plugins.submodules.SharedCode.Systems.Spawn;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests
{
    public class PlayerPredictorTests
    {
        private Contexts contexts = new Contexts();
        private PlayerPredictor playerPredictor;
        private ServerGameEntity playerEntity;

        [SetUp]
        public void SetUp()
        {
            GameSceneFactory gameSceneFactory = new GameSceneFactory();
            Scene matchScene = gameSceneFactory.Create();
            
            ArrangeTransformSystem[] arrangeCollidersSystems = 
            {
                new WithHpArrangeTransformSystem(contexts)
            };
            PhysicsRollbackManager physicsRollbackManager = new PhysicsRollbackManager(arrangeCollidersSystems);

            PhysicsVelocityManager physicsVelocityManager = new PhysicsVelocityManager();
            PhysicsRotationManager physicsRotationManager = new PhysicsRotationManager();
            SnapshotFactory snapshotFactory = new SnapshotFactory(contexts.serverGame);
            playerPredictor = new PlayerPredictor(physicsRollbackManager, matchScene,
                contexts.serverGame, physicsVelocityManager, physicsRotationManager, snapshotFactory);
            
            playerEntity = contexts.serverGame.CreateEntity();
            PrefabsStorage prefabsStorage = new PrefabsStorage();
            GameObject warshipPrefab = prefabsStorage.GetPrefab(ViewTypeEnum.StarSparrow1);
            GameObject go = Object.Instantiate(warshipPrefab, Vector3.zero, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(go, matchScene);
            ushort playerEntityId = 100;
            playerEntity.AddId(playerEntityId);
            playerEntity.AddView(go);
            playerEntity.AddRigidbody(go.GetComponent<Rigidbody>());
            
            LogAssert.ignoreFailingMessages = true;
        }
        
        [TearDown]
        public void PlayerPredictorTestsSimplePasses()
        {
            contexts = new Contexts();
        }

        [UnityTest]
        public IEnumerator Test1()
        {
            LogAssert.ignoreFailingMessages = true;
            InputMessageModel inputMessageModel = new InputMessageModel()
            {
                X = 1,
                Y = 1,
                Angle = float.NaN,
                TickNumber = 0,
                UseAbility = false,
                TickTimeSec = 0
            };
            float deltaTimeSec = 1f;
            Transform goTransform = playerEntity.view.gameObject.transform;
            for (int i = 0; i < 50; i++)
            {
                Vector3 startPosition = goTransform.position;
                playerPredictor.Predict(playerEntity.id.value, inputMessageModel, deltaTimeSec);
                Vector3 finalPosition = goTransform.position;
                float delta = (finalPosition-startPosition).magnitude;
                string message = $"startPosition = {startPosition.x} {startPosition.y} {startPosition.z} | " +
                                 $"finishPosition = {finalPosition.x} {finalPosition.y} {finalPosition.z} " +
                                 $"delta = {delta}";
                Debug.LogWarning(message);
                Assert.IsTrue(delta     > 0.01f);    
            }
            
            yield return null;
        }
    }
}
