using System;
using Code.Prediction;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// Отображает позиции всех обьектов с интерполяцией.
    /// todo игрока и его пули обновлять не нужно
    /// </summary>
    public class UpdateTransformSystem : IExecuteSystem
    {
        private readonly GameContext gameContext;
        private readonly GameStateBuffer gameStateBuffer;
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateTransformSystem));

        public UpdateTransformSystem(Contexts contexts, GameStateBuffer gameStateBuffer)
        {
            gameContext = contexts.game;
            this.gameStateBuffer = gameStateBuffer;
        }

        public void Execute()
        {
            if (!gameStateBuffer.IsReady(out int bufferLength))
            {
                string mes = $"Тик не должен вызыватсья, если gameStateBuffer не готов. " +
                             $"{nameof(bufferLength)} = {bufferLength}";
                throw new Exception(mes);
            }
            
            GameState gameState = gameStateBuffer.GetActualGameState();
            foreach (var pair in gameState.transforms)
            {
                ushort entityId = pair.Key;
                ViewTransform viewTransform = pair.Value;
                GameEntity gameEntity = gameContext.GetEntityWithId(entityId);
                if (gameEntity == null)
                {
                    AddNewObject(entityId, viewTransform);
                }
                else
                {
                    UpdateTransform(gameEntity, viewTransform);
                }
            }
        }

        private void AddNewObject(ushort id, ViewTransform viewTransform)
        {
            GameEntity newObject = gameContext.CreateEntity();
            newObject.AddId(id);
            newObject.AddViewType(viewTransform.viewTypeId);
            newObject.AddTransform(viewTransform.GetPosition(), viewTransform.angle);
        }

        private void UpdateTransform(GameEntity entity, ViewTransform viewTransform)
        {
            Vector3 vector = viewTransform.GetPosition();
            entity.ReplaceTransform(vector, viewTransform.angle);
            ViewTypeId oldViewType = entity.viewType.id;
            if (oldViewType != viewTransform.viewTypeId)
            {
                log.Debug($"Смена типа сущности. Было {oldViewType.ToString()} стало {viewTransform.viewTypeId}");
                entity.ReplaceViewType(viewTransform.viewTypeId);
            }
        }
    }
}