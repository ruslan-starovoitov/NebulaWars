using System.Collections.Generic;
using System.Linq;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public static class Extension
    {
        public static List<List<T>> Split<T>(this IEnumerable<T> source, int count)
        {
            return source
                .Select((x, y) => new { Index = y, Value = x })
                .GroupBy(x => x.Index / count)
                .Select(x => x.Select(y => y.Value).ToList())
                .ToList();
        }
    }
}