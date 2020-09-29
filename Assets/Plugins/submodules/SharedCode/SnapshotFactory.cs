using System.Collections.Generic;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Plugins.submodules.SharedCode
{
    public class SnapshotFactory
    {
        private readonly IGroup<ServerGameEntity> warshipsGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(SnapshotFactory));

        public SnapshotFactory(ServerGameContext context)
        {
            warshipsGroup = context.GetGroup(ServerGameMatcher
                .AllOf(ServerGameMatcher.Transform, ServerGameMatcher.HealthPoints));
        }
        
        public Snapshot Create()
        {
            Dictionary<ushort, ViewTransformCompressed> dictionary = new Dictionary<ushort, ViewTransformCompressed>();
            foreach (var entity in warshipsGroup)
            {
                ushort id = entity.id.value;
                Transform transform = entity.transform.value;
                float angle = entity.transform.value.rotation.eulerAngles.y;
                ViewTypeEnum viewTypeEnum = entity.viewType.value;
                Vector3 position = transform.position;
                var viewTransform = new ViewTransformCompressed(position.x, position.z, angle, viewTypeEnum);
                dictionary.Add(id, viewTransform);
            }
            
            return new Snapshot(dictionary);
        }
    }
}