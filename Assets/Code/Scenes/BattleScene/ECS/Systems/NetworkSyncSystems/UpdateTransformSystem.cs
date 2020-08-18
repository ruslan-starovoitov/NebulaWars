using System.Collections.Generic;
using System.Linq;
using Code.Common.Logger;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Entitas;
using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    /// <summary>
    /// Принимает все состояния мира. Обновляет все transform-ы и создаёт объекты
    /// </summary>
    public class UpdateTransformSystem : IExecuteSystem, ITransformStorage, ITickNumberStorage
    {
        private bool needExecute;
        private int currentTickNumber;
        private readonly GameContext gameContext;
        private readonly object lockObj = new object();
        private readonly IGroup<GameEntity> gameEntitiesGroup;
        private readonly List<PositionsMessage> history = new List<PositionsMessage>();
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateTransformSystem));
        public UpdateTransformSystem(Contexts contexts)
        {
            gameContext = contexts.game;
            gameEntitiesGroup = gameContext.GetGroup(GameMatcher.Transform);
        }

        public void Execute()
        {
            if (!needExecute)
            {
                return;
            }
            
            Dictionary<ushort, ViewTransform> newViewTransforms;
            lock (lockObj)
            {
                newViewTransforms = history.Last().entitiesInfo;
                currentTickNumber = history.Last().TickNumber;
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

        public void SetNewTransforms(in PositionsMessage message)
        {
            lock (lockObj)
            {
                history.Add(message);
                needExecute = true;
            }
        }

        public int GetCurrentTickNumber()
        {
            return currentTickNumber;
        }
    }
}