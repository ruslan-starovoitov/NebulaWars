using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.LagCompensation;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class PhysicsRollbackManager
    {
        private readonly ArrangeTransformSystem[] arrangeTransformSystems;

        public PhysicsRollbackManager(ArrangeTransformSystem[] arrangeTransformSystems)
        {
            this.arrangeTransformSystems = arrangeTransformSystems;
        }
        
        public void Rollback(Snapshot clientSnapshot)
        {
            foreach (var system in arrangeTransformSystems)
            {
                system.Execute(clientSnapshot);
            }
        }
    }
}