using System;

namespace Plugins.submodules.SharedCode
{
    public static class ExceptionExtension
    {
        public static string FullMessage(this Exception exception)
        {
            return exception.Message + " " + exception.StackTrace;
        }
    }
}