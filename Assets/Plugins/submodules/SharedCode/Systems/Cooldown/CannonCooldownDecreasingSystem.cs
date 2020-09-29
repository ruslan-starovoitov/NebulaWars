using Entitas;
using Plugins.submodules.SharedCode.Physics;

namespace Plugins.submodules.SharedCode.Systems.Cooldown
{
    /// <summary>
    /// Обновляет значение перезарядки
    /// </summary>
    public class CannonCooldownDecreasingSystem:IExecuteSystem
    {
        private readonly IGroup<ServerGameEntity> cooldownGroup;
        private readonly ITickDeltaTimeStorage tickDeltaTimeStorage;

        public CannonCooldownDecreasingSystem(Contexts contexts, ITickDeltaTimeStorage tickDeltaTimeStorage)
        {
            this.tickDeltaTimeStorage = tickDeltaTimeStorage;
            cooldownGroup = contexts.serverGame.GetGroup(ServerGameMatcher.CannonCooldown);
        }
        
        public void Execute()
        {
            var entities = cooldownGroup.GetEntities();
            for (var index = 0; index < entities.Length; index++)
            {
                var entity = entities[index];
                float cooldownInSec = entity.cannonCooldown.value;
                if (cooldownInSec > 0)
                {
                    // Debug.LogError("Уменьшение времен перезарядки");
                    entity.cannonCooldown.value = cooldownInSec - tickDeltaTimeStorage.GetDeltaTimeSec();
                }
                else
                {
                    // Debug.LogError("Удаление времени перезарядки");
                    entity.RemoveCannonCooldown();
                }
            }
        }
    }
}