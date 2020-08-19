using System;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.ECS.Systems
{
    public class GameObjectParentsCheckerSystem : IExecuteSystem
    {
        private readonly GameContext gameContext;
        private readonly IGroup<GameEntity> withParents;
        private readonly ILog log = LogManager.CreateLogger(typeof(GameObjectParentsCheckerSystem));
        
        public GameObjectParentsCheckerSystem(Contexts contexts)
        {
            gameContext = contexts.game;
            withParents = gameContext.GetGroup(GameMatcher
                .AllOf(GameMatcher.View, GameMatcher.Parent).NoneOf(GameMatcher.Destroyed));
        }

        public void Execute()
        {
            try
            {
                foreach (GameEntity e in withParents)
                {
                    GameEntity parentEntity = gameContext.GetEntityWithId(e.parent.id);

                    if (parentEntity != null && parentEntity.hasView && !parentEntity.isDestroyed)
                    {
                        e.view.gameObject.transform.SetParent(parentEntity.view.gameObject.transform);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message+" "+e.StackTrace);   
            }
        }
    }
}