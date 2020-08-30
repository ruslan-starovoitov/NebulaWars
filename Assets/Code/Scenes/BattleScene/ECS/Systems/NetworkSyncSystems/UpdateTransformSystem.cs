using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Storages;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// Отображает позиции всех обьектов с интерполяцией.
    /// </summary>
    public class UpdateTransformSystem : IExecuteSystem, IMatchTimeStorage
    {
        private float matchTime;
        private readonly ServerGameContext gameContext;
        private readonly ServerGameStateBuffer serverGameStateBuffer;
        private readonly IGroup<ServerGameEntity> withTransformGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateTransformSystem));

        public UpdateTransformSystem(Contexts contexts, ServerGameStateBuffer serverGameStateBuffer)
        {
            gameContext = contexts.serverGame;
            withTransformGroup = contexts.serverGame
                .GetGroup(ServerGameMatcher.AllOf(ServerGameMatcher.Transform).NoneOf(ServerGameMatcher.HealthBar));
            this.serverGameStateBuffer = serverGameStateBuffer;
        }

        public void Execute()
        {
            if (!serverGameStateBuffer.IsReady(out int bufferLength))
            {
                string mes = $"Тик не должен вызыватсья, если gameStateBuffer не готов. " +
                             $"{nameof(bufferLength)} = {bufferLength}";
                throw new Exception(mes);
            }
            
            HashSet<ushort> ids = new HashSet<ushort>(withTransformGroup
                .GetEntities()
                .Select(item => item.id.value));
            var serializedGameState = serverGameStateBuffer.GetActualGameState(out matchTime);
            foreach (var pair in serializedGameState.transforms)
            {
                ushort entityId = pair.Key;
                var viewTransform = pair.Value;
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

        private void AddNewObject(ushort id, ViewTransformCompressed viewTransform)
        {
            ServerGameEntity gameEntity = gameContext.CreateEntity();
            gameEntity.AddId(id);
            gameEntity.AddViewType(viewTransform.viewTypeEnum);
            Quaternion quaternion = Quaternion.AngleAxis(viewTransform.Angle, Vector3.up);
            gameEntity.AddSpawnTransform(viewTransform.GetPosition(), quaternion);
        }

        private void UpdateTransform(ServerGameEntity entity, ViewTransformCompressed viewTransform)
        {
            ushort playerId = PlayerIdStorage.PlayerEntityId;
            if (entity.id.value == playerId)
            {
                return;
            }
            
            Vector3 vector = viewTransform.GetPosition();
            entity.transform.value.position = vector;
            entity.transform.value.rotation = Quaternion.AngleAxis(viewTransform.Angle, Vector3.up);
            ViewTypeEnum oldViewType = entity.viewType.value;
            if (oldViewType != viewTransform.viewTypeEnum)
            {
                string mes = $"Смена типа сущности. Было {oldViewType.ToString()} стало {viewTransform.viewTypeEnum}";
                log.Debug(mes);
                entity.ReplaceViewType(viewTransform.viewTypeEnum);
            }
        }

        public float GetMatchTimeSec()
        {
            return matchTime;
        }
    }
}