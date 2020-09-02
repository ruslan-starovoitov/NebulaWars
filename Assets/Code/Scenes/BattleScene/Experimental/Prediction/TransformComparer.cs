using System;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Plugins.submodules.SharedCode.Prediction
{
    public class TransformComparer
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(TransformComparer));
        
        public bool IsSame(ViewTransformCompressed viewTransform1, ViewTransformCompressed viewTransform2)
        {
            // bool sameAngle = Math.Abs(viewTransform1.Angle - viewTransform2.Angle) < 1;
            bool sameAngle = true;
            bool sameX = Math.Abs(viewTransform1.X - viewTransform2.X) < 1;
            bool sameZ = Math.Abs(viewTransform1.Z - viewTransform2.Z) < 1;
            bool sameViewType = viewTransform1.viewTypeEnum == viewTransform2.viewTypeEnum;

            if (!sameAngle)
            {
                log.Debug($"!sameAngle = {viewTransform1.Angle} {viewTransform2.Angle}");   
            }
            
            if (!sameX)
            {
                log.Debug($"!sameX = {viewTransform1.X} {viewTransform2.X}");   
            }
            
            if (!sameZ)
            {
                log.Debug($"!sameZ = {viewTransform1.Z} {viewTransform2.Z}");   
            }
            
            if (!sameViewType)
            {
                log.Debug($"!sameViewType = {viewTransform1.viewTypeEnum} {viewTransform2.viewTypeEnum}");   
            }
            
            return sameAngle && sameX && sameZ && sameViewType;
        }
    }
}