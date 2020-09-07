using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Udp.Experimental
{
    /// <summary>
    /// Словарь, которё удаляет старые элементы при превышении максимального размера
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelfCleaningDictionary<T>
    {
        private readonly int maxSizeOfCollection;
        private readonly SortedDictionary<uint, T> history;
        private readonly ILog log = LogManager.CreateLogger(typeof(SelfCleaningDictionary<T>));

        public SelfCleaningDictionary(int maxSizeOfCollection)
        {
            this.maxSizeOfCollection = maxSizeOfCollection;
            history = new SortedDictionary<uint, T>();
        }

        public void Add(uint id, T model)
        {
            history.Add(id, model);
            DeleteOldModels();
        }
        
        private void DeleteOldModels()
        {
            while (history.Count > maxSizeOfCollection)
            {
                history.Remove(history.Keys.Min());
            }
        }

        public Dictionary<uint, T> GetLast(int count)
        {
            IEnumerable<KeyValuePair<uint, T>> max =  history.Skip(history.Count - count).Take(count);
            Dictionary<uint, T> result = new Dictionary<uint, T>();
            foreach (var pair in max)
            {
                result.Add(pair.Key, pair.Value);
            }

            return result;
        }

        public T GetLast()
        {
            return history[history.Keys.Max()];
        }

        public List<KeyValuePair<uint, T>> GetAllFromId(uint id)
        {
            if (id + 1 < history.Keys.Min())
            {
                log.Error($"Запрошен слишком старый ввод. id={id} minId={history.Keys.Min()}");                
            }
            
            List<KeyValuePair<uint, T>> result = history
                .Where(item => item.Key > id)
                .OrderBy(pair=>pair.Key)
                .ToList();
            
            if (id + 1 != result.First().Key)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var inputId in result.Select(pair=>pair.Key))
                {
                    stringBuilder.Append(" " + inputId);
                }
                throw new Exception($"В истории не найдены нужные вводы. " +
                                    $"lastProcessedInputId = {id}" +
                                    $" inputs.First().Key = {stringBuilder}");
            }

            return result;
        }
    }
}