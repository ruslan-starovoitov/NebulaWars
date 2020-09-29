using System.Collections.Generic;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Plugins.submodules.SharedCode
{
    public class Snapshot
    {
        public Dictionary<ushort, ViewTransformCompressed> transforms;

        public Snapshot()
        {
            transforms = new Dictionary<ushort, ViewTransformCompressed>();
        }
        
        public Snapshot(Dictionary<ushort, ViewTransformCompressed> transforms)
        {
            this.transforms = transforms;
        }

        public void Modify(Snapshot snapshot)
        {
            transforms = snapshot.transforms;
        }

        public void Modify(Dictionary<ushort, ViewTransformCompressed> transformsArg)
        {
            transforms = transformsArg;
        }

        public void Clear()
        {
            transforms = new Dictionary<ushort, ViewTransformCompressed>();
        }
    }
}