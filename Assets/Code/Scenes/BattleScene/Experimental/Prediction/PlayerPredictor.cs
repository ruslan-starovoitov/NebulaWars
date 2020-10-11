using Plugins.submodules.EntitasCore.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PlayerPredictor
    {
        private readonly Scene scene;
        private readonly ServerGameContext gameContext;
        private readonly SnapshotFactory snapshotFactory;
        private readonly PhysicsVelocityManager physicsVelocityManager;
        private readonly PhysicsRotationManager physicsRotationManager;
        private readonly PhysicsRollbackManager physicsRollbackManager;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerPredictor));

        public PlayerPredictor(PhysicsRollbackManager physicsRollbackManager, Scene scene,  
            ServerGameContext gameContext, PhysicsVelocityManager physicsVelocityManager,
            PhysicsRotationManager physicsRotationManager, SnapshotFactory snapshotFactory)
        {
            this.scene = scene;
            this.gameContext = gameContext;
            this.snapshotFactory = snapshotFactory;
            this.physicsRollbackManager = physicsRollbackManager;
            this.physicsVelocityManager = physicsVelocityManager;
            this.physicsRotationManager = physicsRotationManager;
        }
        
        /// <summary>
        /// Расставляет все объекты по снимку, применяет ввод к сущности игрока и вызывает тик.
        /// </summary>
        /// <returns></returns>
        public Snapshot Predict(Snapshot snapshot, ushort playerEntityId, InputMessageModel inputMessageModel,
            float physicsSimulationDuration)
        {
            //откатить физическую сцену к состоянию
            physicsRollbackManager.Rollback(snapshot);
            Predict(playerEntityId, inputMessageModel, physicsSimulationDuration);
            log.Debug("Пересимуляция physicsSimulationDuration = "+physicsSimulationDuration);
            return snapshotFactory.Create();
        }

        public void Predict(ushort playerEntityId, InputMessageModel inputMessageModel, float physicsSimulationDuration)
        {
            //взять ввод игрока
            if (inputMessageModel == null)
            {
                log.Debug("Нет ввода");
                return;
            }

            if (physicsSimulationDuration < 0.005f)
            {
                log.Debug("Что за космический fps?");
            }
            
            // log.Debug("Тик физики "+physicsSimulationDuration);
            //линейное движение игрока
            ServerGameEntity playerEntity = gameContext.GetEntityWithId(playerEntityId);
            Rigidbody warshipRigidbody = playerEntity.rigidbody.value;
            Vector3 inputVector = inputMessageModel.GetVector3();
            float maxSpeed = 10f;

            warshipRigidbody.velocity = Vector3.zero;

            if (inputVector.sqrMagnitude > 0.001f)
            {
                // log.Debug("Линейное движение игрока");
                physicsVelocityManager.ApplyVelocity(warshipRigidbody, inputVector, maxSpeed);
            }
            
            //вращательное движение игрока
            if (!float.IsNaN(inputMessageModel.Angle))
            {
                float angularVelocity = 90;
                physicsRotationManager.ApplyRotation(playerEntity.rigidbody.value, inputMessageModel.Angle,
                    angularVelocity, physicsSimulationDuration);
            }

            //todo спавн пуль игрока
            //todo движение пуль игрока
            
            //симуляция физики
            scene.GetPhysicsScene().Simulate(physicsSimulationDuration);
        }
    }
}