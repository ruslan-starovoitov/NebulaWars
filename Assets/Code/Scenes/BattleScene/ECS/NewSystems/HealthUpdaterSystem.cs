using System.Collections.Generic;
using System.Linq;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Health;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    /// <summary>
    /// Обновляет значение хп
    /// </summary>
    public class HealthUpdaterSystem : IExecuteSystem, IHealthPointsStorage
    {
        private bool needUpdate;
        private readonly GameContext gameContext;
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthUpdaterSystem));
        private readonly List<HealthPointsMessagePack> messagePacks = new List<HealthPointsMessagePack>();
        
        public HealthUpdaterSystem(Contexts contexts)
        {
            gameContext = contexts.game;
        }

        public void SetNewHealthPoints(HealthPointsMessagePack message)
        {
            messagePacks.Add(message);
            needUpdate = true;
        }
        
        public void Execute()
        {
            if (!needUpdate)
            {
                return;
            }

            needUpdate = false;
            HealthPointsMessagePack actualValues = messagePacks.Last();
            foreach (var pair in actualValues.entityIdToValue)
            {
                ushort entityId = pair.Key;
                float health = pair.Value;
                var entity = gameContext.GetEntityWithId(entityId);
                if (entity == null)
                {
                    log.Debug("Нет сущности с id "+entityId);
                    continue;
                }

                if (!entity.hasHealth)
                {
                    entity.isNeedHealthBar = true;
                    entity.AddHealth((int)health);
                }
                else if (entity.health.value != (int) health)
                {
                    entity.ReplaceHealth((int)health);
                }
                
                // log.Debug("Обновление хп "+health);
            }
        }
    }
}