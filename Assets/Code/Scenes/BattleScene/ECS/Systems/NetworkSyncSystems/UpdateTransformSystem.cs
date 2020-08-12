using System.Collections.Generic;
using System.Linq;
using Code.Common.Logger;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Entitas;
using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// Хранит все состояния мира
    /// </summary>
    public class UpdateTransformSystem : IExecuteSystem, ITransformStorage
    {
        private bool needExecute;
        private readonly GameContext gameContext;
        private readonly object lockObj = new object();
        private readonly IGroup<GameEntity> gameEntitiesGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateTransformSystem));
        private readonly List<Dictionary<ushort, ViewTransform>> history = new List<Dictionary<ushort, ViewTransform>>();
        
        public UpdateTransformSystem(Contexts contexts)
        {
            gameContext = contexts.game;
            gameEntitiesGroup = gameContext.GetGroup(GameMatcher.Transform);
        }

        public void SetNewTransforms(uint messageId, Dictionary<ushort, ViewTransform> values)
        {
            lock (lockObj)
            {
                history.Add(values);
                needExecute = true;
            }
        }

        public void Execute()
        {
            if (!needExecute)
            {
                return;
            }
            // log.Debug("обработка новых координат");
            
            Dictionary<ushort, ViewTransform> newViewTransforms;
            lock (lockObj)
            {
                newViewTransforms = history.Last();
                needExecute = false;
            }

            //Перебор существующих сущностей
            foreach (var gameEntity in gameEntitiesGroup.GetEntities())
            {
                ushort currentId = gameEntity.id.value;
                // log.Debug("Существующая сущность x "+newViewTransforms[currentId].X+" z "+newViewTransforms[currentId].Z);
                bool entityRemained = newViewTransforms.ContainsKey(currentId);
                if (entityRemained)
                {
                    //Объект остался
                    UpdateTransform(gameEntity, newViewTransforms[currentId]);
                    //Пометка того, что объект обработан
                    newViewTransforms.Remove(currentId);
                }
            }

            //Добавление новых объектов
            foreach (var newEntity in newViewTransforms.OrderBy(pair => pair.Key))
            {
                // log.Debug("новая сущность x "+newEntity.Value.X+" z "+newEntity.Value.Z);
                AddNewObject(newEntity.Key, newEntity.Value);
            }
        }

        private void AddNewObject(ushort id, ViewTransform newTransform)
        {
            var newObject = gameContext.CreateEntity();
            newObject.AddId(id);
            newObject.AddViewType(newTransform.typeId);
            newObject.AddTransform(newTransform.GetPosition(), newTransform.Angle);
        }

        private void UpdateTransform(GameEntity entity, ViewTransform newTransform)
        {
            entity.ReplaceTransform(newTransform.GetPosition(), newTransform.Angle);
            ViewTypeId oldViewType = entity.viewType.id;
            if (oldViewType != newTransform.typeId)
            {
                entity.ReplaceViewType(newTransform.typeId);
            }
        }
    }
}