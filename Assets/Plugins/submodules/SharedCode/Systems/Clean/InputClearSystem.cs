using Entitas;

namespace Plugins.submodules.SharedCode.Systems.Clean
{
    public class InputClearSystem:ICleanupSystem
    {
        private readonly ServerInputContext inputContext;

        public InputClearSystem(Contexts contexts)
        {
            inputContext = contexts.serverInput;
        }
        
        public void Cleanup()
        {
            inputContext.DestroyAllEntities();
        }
    }
}