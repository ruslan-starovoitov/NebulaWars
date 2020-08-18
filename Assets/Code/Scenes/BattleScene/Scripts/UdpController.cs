using Code.Common.Logger;
using Code.Scenes.BattleScene.Udp;
using Code.Scenes.BattleScene.Udp.Connection;
using Code.Scenes.BattleScene.Udp.Experimental;
using System.Net;
using System.Net.Sockets;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS;
using Code.Scenes.BattleScene.Udp.MessageProcessing;
using NetworkLibrary.NetworkLibrary.Http;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Отвечает за создание/остановку udp соединения.
    /// </summary>
    public class UdpController : MonoBehaviour
    {
        private UdpSendUtils udpSendUtils;
        private UdpClientWrapper udpClientWrapper;
        private readonly ILog log = LogManager.CreateLogger(typeof(UdpController));

        private void Awake()
        {
            var matchSimulation = FindObjectOfType<MatchSimulation>();
            
            //Если в прошлом бою уже был создан UdpClient
            udpClientWrapper?.Stop();
            
            BattleRoyaleClientMatchModel matchData = MatchModelStorage.Instance.GetMatchModel();
            if (matchData == null)
            {
                log.Error("Нет данных о матче. Симуляция не работает.");
                return;
            }
            int matchId = matchData.MatchId;
            int gameServerPort = matchData.GameServerPort;
            string gameServerIp = matchData.GameServerIp;
            IPEndPoint serverIpEndPoint = new IPEndPoint(IPAddress.Parse(gameServerIp), gameServerPort);
            ByteArrayHandler byteArrayHandler = new ByteArrayHandler();
            
            log.Info("Установка прослушки udp.");
            UdpClient udpClient = new UdpClient
            {
                Client =
                {
                    Blocking = false
                }
            };
            udpClient.Connect(serverIpEndPoint);
            udpClientWrapper = new UdpClientWrapper(udpClient, byteArrayHandler);
            udpSendUtils = new UdpSendUtils(matchId, udpClientWrapper);
            MessageWrapperHandler messageWrapperHandler = new MessageWrapperHandler(udpSendUtils, matchId,
                matchSimulation, matchSimulation, matchSimulation, matchSimulation);
            byteArrayHandler.Initialize(messageWrapperHandler);
            udpClientWrapper.StartReceiveThread();   
        }
        
        public UdpSendUtils GetUdpSendUtils()
        {
            return udpSendUtils;
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