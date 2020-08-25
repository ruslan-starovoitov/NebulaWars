using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scenes.BattleScene.Udp.MessageProcessing;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Health;

namespace Code.Scenes.BattleScene.ECS.NewSystems
{
    public class MaxHealthUpdaterSystem : IExecuteSystem, IMaxHealthPointsMessagePackStorage
    {
        private bool needUpdate;
        private readonly ServerGameContext gameContext;
        private readonly ILog log = LogManager.CreateLogger(typeof(MaxHealthUpdaterSystem));
        private readonly List<MaxHealthPointsMessagePack> messages = new List<MaxHealthPointsMessagePack>();
        
        public MaxHealthUpdaterSystem(Contexts contexts)
        {
            gameContext = contexts.serverGame;
        }

        public void SetNewMaxHealthPoints(MaxHealthPointsMessagePack message)
        {
            // log.Debug("Кол-во объектов с хп "+message.entityIdToValue.Count);
            messages.Add(message);
            needUpdate = true;
        }
        
        public void Execute()
        {
            if (!needUpdate)
            {
                return;
            }

            needUpdate = false;
            var actualValues = messages.Last();
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

                if (!entity.hasMaxHealthPoints)
                {
                    entity.AddMaxHealthPoints((int)health);
                }
                else if (Math.Abs(entity.maxHealthPoints.value - health) > 0.01f)
                {
                    entity.ReplaceMaxHealthPoints((int) health);
                }
                // log.Debug("Обновление хп "+health);
            }
        }

       
    }
}