using System.Collections.Generic;
using System.Linq;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    /// <summary>
    /// Словарь, которё удаляет старые элементы при превышении максимального размера
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelfCleaningDictionary<T>
    {
        private readonly int maxSizeOfCollection;
        private readonly Dictionary<uint, T> history;

        public SelfCleaningDictionary(int maxSizeOfCollection)
        {
            this.maxSizeOfCollection = maxSizeOfCollection;
            history = new Dictionary<uint, T>(maxSizeOfCollection);
        }

        public void Add(uint id, T model)
        {
            history.Add(id, model);
            DeleteOldModels();
        }
        
        private void DeleteOldModels()
        {
            if (history.Count > maxSizeOfCollection)
            {
                var redundant = history
                    .OrderByDescending(pair=>pair.Key)
                    .Skip(maxSizeOfCollection);
                foreach (var keyValuePair in redundant)
                {
                    var tmpInputId = keyValuePair.Key;
                    history.Remove(tmpInputId);
                }
            }
        }

        public Dictionary<uint, T> Read()
        {
            return history;
        }

        public T GetLast()
        {
            return history[history.Keys.Max()];
        }
    }
}