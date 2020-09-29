using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Plugins.submodules.SharedCode.Logger
{
    /// <summary>
    /// Тут два одинаковых метода потому, что с одним методом с аргументом
    /// IEnumerable<string> оно ругалось на возможные аллокации.
    /// </summary>
    public class LogPrinter
    {
        private readonly string path;
        
        public LogPrinter(string persistentDataPath)
        {
            path = $"{persistentDataPath}/logs.txt";
        }
        
        public void Print(List<string> lines)
        {
            using (var sw = new StreamWriter(path, true, Encoding.UTF8))
            {
                sw.WriteLine("=========START PRINT=========");
                foreach (string line in lines)
                {
                    sw.WriteLine(line);
                }
                sw.WriteLine("=========END PRINT=========");
            }
        }
        
        public void Print(string[] lines)
        {
            using (var sw = new StreamWriter(path, true, Encoding.UTF8))
            {
                sw.WriteLine("=========START PRINT=========");
                foreach (string line in lines)
                {
                    sw.WriteLine(line);
                }
                sw.WriteLine("=========END PRINT=========");
            }
        }
    }
}