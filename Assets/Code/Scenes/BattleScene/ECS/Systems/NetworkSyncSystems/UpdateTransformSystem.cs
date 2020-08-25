using System;
using System.Collections.Generic;
using System.Linq;
using Code.Prediction;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// Отображает позиции всех обьектов с интерполяцией.
    /// </summary>
    public class UpdateTransformSystem : IExecuteSystem
    {
        private readonly ServerGameContext gameContext;
        private readonly GameStateBuffer gameStateBuffer;
        private readonly IGroup<ServerGameEntity> withTransformGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateTransformSystem));

        public UpdateTransformSystem(Contexts contexts, GameStateBuffer gameStateBuffer)
        {
            gameContext = contexts.serverGame;
            withTransformGroup = contexts.serverGame
                .GetGroup(ServerGameMatcher.AllOf(ServerGameMatcher.Transform).NoneOf(ServerGameMatcher.HealthBar));
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
            
            HashSet<ushort> ids = new HashSet<ushort>(withTransformGroup
                .GetEntities()
                .Select(item => item.id.value));
            GameState gameState = gameStateBuffer.GetActualGameState();
            foreach (var pair in gameState.transforms)
            {
                ushort entityId = pair.Key;
                ViewTransform viewTransform = pair.Value;
                ServerGameEntity gameEntity = gameContext.GetEntityWithId(entityId);
                if (gameEntity == null)
                {
                    AddNewObject(entityId, viewTransform);
                }
                else
                {
                    UpdateTransform(gameEntity, viewTransform);
             
                    ids.Remove(gameEntity.id.value);
                }
            }

            foreach (ushort id in ids)
            {
                // log.Debug($"Удаление объекта id = {id}");
                ServerGameEntity gameEntity = gameContext.GetEntityWithId(id);
                gameEntity.isDestroyed = true;
            }
        }

        private void AddNewObject(ushort id, ViewTransform viewTransform)
        {
            ServerGameEntity gameEntity = gameContext.CreateEntity();
            gameEntity.AddId(id);
            gameEntity.AddViewType(viewTransform.viewTypeEnum);

            Quaternion quaternion = Quaternion.AngleAxis(viewTransform.angle, Vector3.up);
            gameEntity.AddSpawnTransform(viewTransform.GetPosition(), quaternion);
        }

        private void UpdateTransform(ServerGameEntity entity, ViewTransform viewTransform)
        {
            Vector3 vector = viewTransform.GetPosition();
            entity.transform.value.position = vector;
            entity.transform.value.rotation = Quaternion.AngleAxis(viewTransform.angle, Vector3.up);
            ViewTypeEnum oldViewType = entity.viewType.value;
            if (oldViewType != viewTransform.viewTypeEnum)
            {
                string mes = $"Смена типа сущности. Было {oldViewType.ToString()} стало {viewTransform.viewTypeEnum}";
                log.Debug(mes);
                entity.ReplaceViewType(viewTransform.viewTypeEnum);
            }
        }
    }
}