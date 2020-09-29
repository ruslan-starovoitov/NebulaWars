using System;
using System.Collections.Generic;
using Plugins.submodules.SharedCode.Logger;

namespace Plugins.submodules.SharedCode.Systems
{
    /// <summary>
    /// Нужен для определения того, какие сообщения уже были обработаны, а какие нужно обработать.
    /// </summary>
    public class InputMessagesMetaHistory:ILastProcessedInputIdStorage
    {
        private readonly int maxHashSetLength;
        //tmpPlayerId inputMessageIds 
        private readonly Dictionary<ushort, SortedSet<uint>> dictionary;
        private readonly ILog log = LogManager.CreateLogger(typeof(InputMessagesMetaHistory));
        
        public InputMessagesMetaHistory(int maxHashSetLength, List<ushort> playerIds)
        {
            this.maxHashSetLength = maxHashSetLength;
            dictionary = new Dictionary<ushort, SortedSet<uint>>(playerIds.Count);
            foreach (ushort playerId in playerIds)
            {
                dictionary.Add(playerId, new SortedSet<uint>());
            }
        }

        public bool TryAddId(ushort playerId, uint inputMessageId)
        {
            if (!dictionary.ContainsKey(playerId))
            {
                log.Error("Сообщения от этого игрока не ожидаются");
                return false;
            }

            SortedSet<uint> sortedSet = dictionary[playerId];
            if (sortedSet.Contains(inputMessageId))
            {
                return false;
            }
            
            sortedSet.Add(inputMessageId);
            if (sortedSet.Count > maxHashSetLength)
            {
                sortedSet.Remove(sortedSet.Min);
            }
            
            //Сообщение очень старое
            if (inputMessageId < sortedSet.Min)
            {
                return false;
            }

            return true;
        }
        
        public uint? Get(ushort tmpPlayerId)
        {
            var set = dictionary[tmpPlayerId];
            if (set.Count == 0)
            {
                return null;
            }
            else
            {
                uint result = set.Max;
                return result;
            }
        }
    }
}