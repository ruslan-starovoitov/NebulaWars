using System.Collections.Generic;
using Code.Scenes.BattleScene.Experimental;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public class UpdateDestroysSystem : IExecuteSystem
    {
        private static HashSet<ushort> destroys;
        private readonly IGroup<GameEntity> positionedGroup;
        private static readonly object LockObj = new object();
        private const float TimeDelay = ClientTimeManager.TimeDelay;
        private readonly ILog log = LogManager.CreateLogger(typeof(UpdateDestroysSystem));

        public UpdateDestroysSystem(Contexts contexts)
        {
            destroys = new HashSet<ushort>();
            var matcher = GameMatcher.AllOf(GameMatcher.Transform, GameMatcher.View);
            positionedGroup = contexts.game.GetGroup(matcher);
        }

        public static void SetNewDestroys(ushort[] newDestroys)
        {
            lock (LockObj)
            {
                destroys.UnionWith(newDestroys);
            }
        }

        public void Execute()
        {
            lock (LockObj)
            {
                foreach (var entity in positionedGroup)
                {
                    var id = entity.id.value;
                    if (destroys.Contains(id))
                    {
                        if (entity.hasDelayedDestroy)
                        {
                            log.Error($"Сообщение об удалении объекта с id {id} пришло несколько раз.");
                        }
                        else
                        {
                            entity.AddDelayedDestroy(TimeDelay);
                        }
                        destroys.Remove(id);
                    }
                }
            }
        }
    }
}