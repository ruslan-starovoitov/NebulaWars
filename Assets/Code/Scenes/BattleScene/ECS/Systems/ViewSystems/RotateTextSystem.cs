using System;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.ViewSystems
{
    public class RotateTextSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> withText;
        private readonly ILog log = LogManager.CreateLogger(typeof(RotateTextSystem));
        
        public RotateTextSystem(Contexts contexts)
        {
            withText = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.View, GameMatcher.TextMeshPro).NoneOf(GameMatcher.Hidden));
        }

        public void Execute()
        {
            try
            {
                foreach (var e in withText)
                {
                    var tmp = e.textMeshPro.value;

                    tmp.transform.rotation = Quaternion.identity;
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message+" "+e.StackTrace);
            }
        }
    }
}