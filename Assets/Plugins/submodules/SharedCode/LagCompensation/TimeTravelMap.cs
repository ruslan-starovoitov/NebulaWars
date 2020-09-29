using System.Collections.Generic;
using System.Linq;

namespace Plugins.submodules.SharedCode.LagCompensation
{
    /// <summary>
    /// Разбивает игровое соостояие на наборы, которые нужно обработать в разных тиках
    /// </summary>
    public class TimeTravelMap
    {
        public class Bucket
        {
            public int TickNumber { get; }
            /// <summary>
            /// Сущности с снарядами
            /// </summary>
            public readonly List<ServerGameEntity> GameEntities = new List<ServerGameEntity>();

            public Bucket(int tickNumber)
            {
                TickNumber = tickNumber;
            }
        }

        ///На вход кластеризатор принимает текущее игровое состояние,
        ///а на выход выдает набор «корзин». В каждой корзине лежат сущности,
        ///которым для лагкомпенсации нужно одно и то же время из истории.
        public List<Bucket> RefillBuckets(Contexts contexts)
        {
            Dictionary<int, Bucket> buckets = new Dictionary<int, Bucket>();
            var needTickEntities = contexts.serverGame
                .GetGroup(ServerGameMatcher.TickNumber)
                .GetEntities();

            foreach (var entity in needTickEntities)
            {
                int tick = entity.tickNumber.value;
                if (!buckets.ContainsKey(tick))
                {
                    buckets.Add(tick, new Bucket(tick));
                }
                
                buckets[tick].GameEntities.Add(entity);
            }
            
            return buckets.Values.ToList();
        }
    }
}