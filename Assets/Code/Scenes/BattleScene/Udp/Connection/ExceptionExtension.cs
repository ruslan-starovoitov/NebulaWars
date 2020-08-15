using System;

namespace Code.Scenes.BattleScene.Udp.Connection
{
    public static class ExceptionExtension
    {
        public static string FullMessage(this Exception exception)
        {
            return exception.Message + " " + exception.StackTrace;
        }
    }
}