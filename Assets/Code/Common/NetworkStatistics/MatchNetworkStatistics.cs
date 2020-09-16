#define ENABLE_NETWORK_LOGS
// #undef ENABLE_NETWORK_LOGS

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Scenes.BattleScene.Scripts;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.Utils;
using UnityEngine;

namespace Code.Common.NetworkStatistics
{
    /// <summary>
    /// Хранит информацию о статистике сети в бою.
    /// </summary>
    public class MatchNetworkStatistics
    {
        private readonly int matchId;
        private readonly ushort playerTmpId;
        private readonly object lockObj = new object();
        private readonly List<int> datagramIds = new List<int>(30*40);
        private readonly ILog log = LogManager.CreateLogger(typeof(MatchNetworkStatistics));
        
        private readonly List<DatagramPerSecondModel> packetsCount = new List<DatagramPerSecondModel>(){new DatagramPerSecondModel()
        {
            pps = 0,
            dateTime = DateTime.UtcNow
        }};
        private int prevSec;
        private int fps;
        
        /// <summary>
        /// Длина сообщения, количество дейтаграм такой длины
        /// </summary>
        private readonly List<MessageRecord> allMessages = new List<MessageRecord>(1000);
        private readonly Dictionary<int,int> datagramLengthAndCount = new Dictionary<int, int>();
        private readonly Dictionary<MessageType, long> messageTypeTotalSize = new Dictionary<MessageType, long>();
        
        public MatchNetworkStatistics(int matchId, ushort playerTmpId)
        {
            this.matchId = matchId;
            this.playerTmpId = playerTmpId;
        }
        
        public void RegisterMessage(int messageLength, MessageType messageType)
        {
#if ENABLE_NETWORK_LOGS
            if (messageTypeTotalSize.TryGetValue(messageType, out var value))
            {
                messageTypeTotalSize[messageType] = value + messageLength;
            }
            else
            {
                messageTypeTotalSize.Add(messageType, messageLength);
            }
            
            allMessages.Add(new MessageRecord
            {
                Length = messageLength,
                MessageType = messageType
            });
#endif
        }

        public int GetLastFramerate()
        {
            lock (lockObj)
            {
                if (packetsCount.Count >= 2)
                {
                    return packetsCount[packetsCount.Count - 2].pps;
                }
    
            }
            
            return 0;
        }
        
        public void RegisterDatagram(int datagramLength, int datagramId)
        {
#if ENABLE_NETWORK_LOGS
            lock (lockObj)
            {
                int lastValue = packetsCount[packetsCount.Count-1].pps;
                packetsCount[packetsCount.Count-1].pps = lastValue + 1;
                packetsCount[packetsCount.Count - 1].totalLength += datagramLength;
            
                int sec = DateTime.UtcNow.Second;
                if (sec != prevSec)
                {
                    packetsCount.Add(new DatagramPerSecondModel()
                    {
                        dateTime = DateTime.UtcNow,
                        pps = 0,
                        totalLength = 0
                    });
                    prevSec = sec;
                }    
            }
            
            if (datagramIds.Count > 1)
            {
                int lastId = datagramIds.Last();
                if (lastId + 1 != datagramId)
                {
                    DatagramsOrderLog.DatagramsError();
                }

                if (datagramId < lastId)
                {
                    string message = $"Сообщения пришли в неправильном порядке " +
                                     $"{nameof(lastId)} {lastId} " +
                                     $"{nameof(datagramId)} {datagramId}";
                    log.Info(message);
                }
            }
            
            datagramIds.Add(datagramId);
            
            if (datagramLengthAndCount.TryGetValue(datagramLength, out var value))
            {
                datagramLengthAndCount[datagramLength] = value + 1;
            }
            else
            {
                datagramLengthAndCount.Add(datagramLength, 1);
            }
            
#endif
        }

        /// <summary>
        /// Нарушение Single responsibility
        /// </summary>
        public void Print()
        {
#if ENABLE_NETWORK_LOGS
            lock (lockObj)
            {
                string dateTime = DateTime.Now.ToLongTimeString().Replace(':', '_');
                string fileName = $"networkStatistics_{matchId}_{dateTime}.txt";
                fileName = Application.persistentDataPath +"/"+ fileName;
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.WriteLine($"{nameof(matchId)} {matchId}");
                    sw.WriteLine($"{nameof(playerTmpId)} {playerTmpId}");
                    
                    sw.WriteLine();
                    sw.WriteLine();

                    sw.WriteLine("Дейтаграммы:");
                    sw.WriteLine("Время || кол-во датаграмм || длина в байтах");
                    foreach (DatagramPerSecondModel model in packetsCount)
                    {
                        sw.WriteLine($"{model.dateTime.ToLongTimeString()}\t\t {model.pps}\t\t {model.totalLength}");
                    }
                    
                    sw.WriteLine();
                    sw.WriteLine();
                    
                    sw.WriteLine();
                    sw.WriteLine("Номера дейтаграмм c нарушением порядка");

                    int tmpDatagramId = 0;
                    foreach (int datagramId in datagramIds)
                    {
                        if (tmpDatagramId + 1 != datagramId)
                        {
                            sw.WriteLine(datagramId+" <- нарушение порядка");
                        }

                        tmpDatagramId = datagramId;
                    }

                    int maxDatagramId = datagramIds.Max();
                    HashSet<int> allNumbers = new HashSet<int>();
                    for (int i = 0; i <= maxDatagramId; i++)
                    {
                        allNumbers.Add(i);
                    }

                    foreach (var datagramId in datagramIds)
                    {
                        allNumbers.Remove(datagramId);
                    }

                    int datagramsLossCount = allNumbers.Count;
                    float datagramLossPercentage = 1f*datagramsLossCount / maxDatagramId;
                    sw.WriteLine($"datagramsLossCount {datagramsLossCount}");
                    sw.WriteLine($"maxDatagramId {maxDatagramId}");
                    sw.WriteLine($"datagram loss {datagramLossPercentage}%");
                    
                    sw.WriteLine();
                    sw.WriteLine();
                    
                    
                    
                    sw.WriteLine($"Общий размер переданных сообщений в байтах= {datagramLengthAndCount.Sum(pair => pair.Key*pair.Value)}");
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine("Статистика по типам сообщений (тип и размер всех сообщений типа в байтах)");
                    foreach (KeyValuePair<MessageType, long> pair in messageTypeTotalSize)
                    {
                        sw.WriteLine($"{pair.Key.ToString()} \t\t\t {pair.Value}");
                    }
                    
                    List<int> sortedKeys = datagramLengthAndCount.Keys.ToList();
                    sortedKeys.Sort();
                    sw.WriteLine("Статистика по размеру дейтаграм с округлением (размер дейтаграм и их количество) сортировка по размеру");
                    for (int maxLength = 100; maxLength < 2000; maxLength+=100)
                    {
                        int count = sortedKeys.Where(key => maxLength - 100< key&& key <= maxLength)
                            .Select(key=>datagramLengthAndCount[key])
                            .Sum(value=>value);
                        sw.WriteLine($"{maxLength-100}-{maxLength} {count}");
                    }
                    
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine();
                    sw.WriteLine("Все сообщения");
                    lock (lockObj)
                    {
                        foreach (string record in allMessages.Select(item=>item.ToString()))
                        {
                            sw.WriteLine(record);
                        }    
                    }
                }
            }
#endif
        }
    }
}