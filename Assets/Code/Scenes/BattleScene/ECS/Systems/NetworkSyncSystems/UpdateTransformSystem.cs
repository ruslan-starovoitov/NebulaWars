using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common.Storages;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;
using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public class MatchTimeIsTooShort:Exception
    {
        
    }
    
    public class MatchTimeIsTooLong:Exception
    {
        
    }
    /// <summary>
    /// Отображает позиции всех обьектов с интерполяцией.
    /// </summary>
    public class UpdateTransformSystem : IExecuteSystem
    {
        private readonly ServerGameContext gameContext;
        private readonly ISnapshotManager snapshotManager;
        private readonly IMatchTimeStorage matchTimeStorage;
        private readonly IGroup<ServerGameEntity> withTransformGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateTransformSystem));

        public UpdateTransformSystem(Contexts contexts, ISnapshotManager snapshotManager,
            IMatchTimeStorage matchTimeStorage)
        {
            this.snapshotManager = snapshotManager;
            this.matchTimeStorage = matchTimeStorage;
            gameContext = contexts.serverGame;
            var matcher = ServerGameMatcher.AllOf(ServerGameMatcher.Transform).NoneOf(ServerGameMatcher.HealthBar);
            withTransformGroup = contexts.serverGame.GetGroup(matcher);
        }

        public void Execute()
        {
            HashSet<ushort> ids = new HashSet<ushort>(withTransformGroup
                .GetEntities()
                .Select(item => item.id.value));
            
            float matchTime = matchTimeStorage.GetMatchTime();

            Snapshot snapshot;
            try
            {
                snapshot = snapshotManager.CreateInterpolatedSnapshot(matchTime);
            }
            catch (Exception e)
            {
                log.Error(e.FullMessage());
                return;
            }

            if (snapshot == null)
            {
                throw new NullReferenceException($"snapshot is null");
            }
            
            foreach (var pair in snapshot.transforms)
            {
                ushort entityId = pair.Key;
                ViewTransformCompressed viewTransform = pair.Value;
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
    }
}