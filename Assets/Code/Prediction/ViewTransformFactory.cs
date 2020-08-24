using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Prediction
{
    public static class ViewTransformFactory
    {
        public static ViewTransform Create(ViewTransformCompressed viewTransformCompressed)
        {
            float x = viewTransformCompressed.X;
            float z = viewTransformCompressed.Z;
            float angle = viewTransformCompressed.Angle;
            ViewTypeEnum type = viewTransformCompressed.viewType;
            return new ViewTransform(x, z, angle, type);
        }
    }
}