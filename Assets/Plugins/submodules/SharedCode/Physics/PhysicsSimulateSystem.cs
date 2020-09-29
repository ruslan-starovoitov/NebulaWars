using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Plugins.submodules.SharedCode.Physics
{
    /// <summary>
    /// Вызывает обработку физики
    /// </summary>
    public class PhysicsSimulateSystem : IExecuteSystem
    {
        private readonly Scene scene;
        private readonly ITickDeltaTimeStorage tickDeltaTimeStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(PhysicsSimulateSystem));

        public PhysicsSimulateSystem(Scene scene, ITickDeltaTimeStorage tickDeltaTimeStorage)
        {
            this.scene = scene;
            this.tickDeltaTimeStorage = tickDeltaTimeStorage;
        }

        public void Execute()
        {
            float deltaTime = tickDeltaTimeStorage.GetDeltaTimeSec();
            scene.GetPhysicsScene().Simulate(deltaTime);
        }
    }
}