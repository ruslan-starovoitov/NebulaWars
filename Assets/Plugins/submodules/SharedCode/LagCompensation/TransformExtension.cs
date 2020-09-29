using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using UnityEngine;

namespace Plugins.submodules.SharedCode.LagCompensation
{
    public static class TransformExtension
    {
        public static void Change(this Transform transform, ViewTransformCompressed viewTransform)
        {
            transform.position = new Vector3(viewTransform.X, 0, viewTransform.Z);
            transform.rotation = Quaternion.AngleAxis(viewTransform.Angle, Vector3.up);
        }
    }
}