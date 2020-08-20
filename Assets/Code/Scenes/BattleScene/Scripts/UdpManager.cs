using System;
using Code.Scenes.BattleScene.Udp.Connection;
using Code.Scenes.BattleScene.Udp.Experimental;
using System.Net;
using System.Net.Sockets;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Отвечает за создание/остановку udp соединения.
    /// </summary>
    public class UdpManager : MonoBehaviour
    {
        private UdpClientWrapper udpClientWrapper;
        private readonly ILog log = LogManager.CreateLogger(typeof(UdpManager));

        private void Awake()
        {
            //Если в прошлом бою уже был создан UdpClient
            udpClientWrapper?.Stop();
        }

        public UdpSendUtils CreateConnection(BattleRoyaleClientMatchModel matchData)
        {
            if (matchData == null)
            {
                throw new Exception("Нет данных о матче. Симуляция не работает.");
            }
            
            int matchId = matchData.MatchId;
            int gameServerPort = matchData.GameServerPort;
            string gameServerIp = matchData.GameServerIp;
            IPEndPoint serverIpEndPoint = new IPEndPoint(IPAddress.Parse(gameServerIp), gameServerPort);
            
            
            log.Info("Установка прослушки udp.");
            UdpClient udpClient = new UdpClient
            {
                Client =
                {
                    Blocking = false
                }
            };
            udpClient.Connect(serverIpEndPoint);
            udpClientWrapper = new UdpClientWrapper(udpClient);
            UdpSendUtils udpSendUtils = new UdpSendUtils(matchId, udpClientWrapper);
            return udpSendUtils;
        }

        public void StartListening(IByteArrayHandler byteArrayHandler)
        {
            udpClientWrapper.StartReceiveThread(byteArrayHandler);
        }
        
        /// <summary>
        /// Тут не нужно закрывать соединение потому, что сообщения могут
        ///  отправляться/приниматься ещё несколько секунд
        /// </summary>
        private void OnDestroy()
        {
            udpClientWrapper?.Stop();
            udpClientWrapper?.Dispose();
        }
    }
}