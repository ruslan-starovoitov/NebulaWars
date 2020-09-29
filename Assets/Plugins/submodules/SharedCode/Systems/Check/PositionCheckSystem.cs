using System;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.Check
{
    public class PositionCheckSystem : IExecuteSystem
    {
        private readonly IGroup<ServerGameEntity> transformGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(PositionCheckSystem));

        public PositionCheckSystem(Contexts contexts)
        {
            transformGroup = contexts.serverGame.GetGroup(ServerGameMatcher.Transform);
        }
        
        public void Execute()
        {
            foreach (var entity in transformGroup)
            {
                try
                {
                    if (Mathf.Abs(entity.transform.value.position.y) > 0.3f)
                    {
                        var position = entity.transform.value.position;
                        Debug.LogError($"entity.hasPlayer = " + entity.hasPlayer);
                        Debug.LogError($"position = {position.x} {position.y} {position.z}");
                        Debug.LogError($"position = {position.x} {position.y} {position.z}");
                        throw new Exception("y != 0 " + position.y);
                    }
                }
                catch (Exception e)
                {
                    ushort id = entity.id.value;
                    log.Error(e.FullMessage()+"entity id = "+id);
                }
            }    
        }
    }
}