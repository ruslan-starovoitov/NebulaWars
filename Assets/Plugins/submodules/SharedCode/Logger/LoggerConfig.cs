using JetBrains.Annotations;

namespace Plugins.submodules.SharedCode.Logger
{
    public class LoggerConfig
    {
        /// <summary>
        /// Если наберётся такое кол-во сообщений, то логгер попытается дописать их в файл.
        /// </summary>
        public int MaxNumberOfRecordsToPrint { get; }
        /// <summary>
        /// Раз с n миллисекунд логгер будет дописывать сообщения в файл, если они есть. 
        /// </summary>
        public int MaxPrintingPeriodMs{ get; }
        /// <summary>
        /// Путь по которому будет сохранен logs.txt
        /// </summary>
        [NotNull] public string PersistentDataPath{ get; }
        
        public LoggerConfig(int maxNumberOfRecordsToPrint, int maxPrintingPeriodMs, string persistentDataPath)
        {
            MaxNumberOfRecordsToPrint = maxNumberOfRecordsToPrint;
            MaxPrintingPeriodMs = maxPrintingPeriodMs;
            PersistentDataPath = persistentDataPath;
        }

        public override string ToString()
        {
            return $"Параметры логгера {nameof(PersistentDataPath)} {PersistentDataPath} " +
                   $"{nameof(MaxPrintingPeriodMs)} {MaxPrintingPeriodMs}" +
                   $"{nameof(MaxNumberOfRecordsToPrint)} {MaxNumberOfRecordsToPrint}";
        }
    }
}