using System;
using System.Collections.Generic;
using System.Linq;
using NetworkLibrary.NetworkLibrary.Udp;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Common.NetworkStatistics
{
    /// <summary>
    /// Отвечает за запись статистики сети для всех боёв
    /// </summary>
    public class NetworkStatisticsStorage
    {
        private MatchNetworkStatistics lastMatch;
        public static NetworkStatisticsStorage Instance => Lazy.Value;
        private readonly ILog log = LogManager.CreateLogger(typeof(NetworkStatisticsStorage));
        private readonly List<MatchNetworkStatistics> matches = new List<MatchNetworkStatistics>();
        private static readonly Lazy<NetworkStatisticsStorage> Lazy = new Lazy<NetworkStatisticsStorage> (() => new NetworkStatisticsStorage()); 

        public void StartRecordingNewMatch(int matchId, ushort playerTmpId)
        {
            lastMatch = new MatchNetworkStatistics(matchId, playerTmpId);
            matches.Add(lastMatch);
        }

        public void RegisterMessage(int messageLength, MessageType messageType)
        {
            if (lastMatch == null)
            {
                throw new Exception($"Перед добавлением сообщения нужно вызвать {nameof(StartRecordingNewMatch)}");
            }
        
            lastMatch.RegisterMessage(messageLength, messageType);
        }

        public int GetLastFramerate()
        {
            return lastMatch.GetLastFramerate();
        }
        
        public void RegisterDatagram(int datagramLength, int datagramId)
        {
            if (lastMatch == null)
            {
                throw new Exception($"Перед добавлением сообщения нужно вызвать {nameof(StartRecordingNewMatch)}");
            }

            lastMatch.RegisterDatagram(datagramLength, datagramId);
        }

        public void PrintSavedMatches()
        {
            foreach (MatchNetworkStatistics match in matches)
            {
                match.Print();
            }
            matches.Clear();
            lastMatch = null;
        }
    }
}