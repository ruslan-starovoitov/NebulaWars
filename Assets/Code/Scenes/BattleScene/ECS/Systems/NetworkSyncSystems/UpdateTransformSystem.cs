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
    /// Принимает все состояния мира. Обновляет все transform-ы и создаёт объекты
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


            var gameEntities = gameEntitiesGroup.GetEntities();
            if (gameEntities == null)
            {
                log.Debug("gameEntities is null");
                return;
            }
            
            if (gameEntities.Length ==  0)
            {
                log.Debug("gameEntities is empty");
            }


            //Перебор существующих сущностей
            foreach (var gameEntity in gameEntitiesGroup.GetEntities())
            {
                ushort currentId = gameEntity.id.value;
                // log.Debug("Существующая сущность currentId "+currentId);
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
            var test = newViewTransforms.OrderBy(pair => pair.Key);

            
            // log.Debug("test.Count() "+test.Count());
            
                
            foreach (var newEntity in test)
            {
                // log.Debug("новая сущность  id "+newEntity.Key);
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
            var vector = newTransform.GetPosition();
            entity.ReplaceTransform(vector, newTransform.Angle);
            ViewTypeId oldViewType = entity.viewType.id;
            if (oldViewType != newTransform.typeId)
            {
                entity.ReplaceViewType(newTransform.typeId);
            }
        }
    }
}