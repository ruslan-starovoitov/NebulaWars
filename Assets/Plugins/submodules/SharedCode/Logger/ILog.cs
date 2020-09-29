namespace Plugins.submodules.SharedCode.Logger
{
    public interface ILog
    {
        void Debug(string message);
        void Debug(object message);
        
        void Info(string message);
        void Info(object message);
        
        void Warn(string message);
        void Warn(object message);
        
        void Error(string message);
        void Error(object message);

        void Print();
    }
}