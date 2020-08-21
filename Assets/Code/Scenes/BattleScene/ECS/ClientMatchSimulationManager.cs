using Code.Common.Storages;
using Code.Scenes.BattleScene.Scripts;
using Code.Scenes.BattleScene.Scripts.Ui;
using Code.Scenes.BattleScene.Udp;
using Code.Scenes.BattleScene.Udp.Experimental;
using Code.Scenes.BattleScene.Udp.MessageProcessing;
using NetworkLibrary.NetworkLibrary.Http;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    public class ClientMatchSimulationManager : MonoBehaviour
    {
        private UdpManager udpManager;
        private MatchModelStorage matchModelStorage;
        private BattleUiController battleUiController;
        private ClientMatchSimulation clientMatchSimulation;

        private void Awake()
        {
            battleUiController = FindObjectOfType<BattleUiController>();
            udpManager = FindObjectOfType<UdpManager>();
            matchModelStorage = MatchModelStorage.Instance;
        }

        private void Start()
        {
            BattleRoyaleClientMatchModel matchModel = matchModelStorage.GetMatchModel();
            UdpSendUtils udpSendUtils = udpManager.CreateConnection(matchModel);

            clientMatchSimulation = new ClientMatchSimulation(battleUiController, udpSendUtils, matchModel);
            
            var playersStorage = clientMatchSimulation.GetIPlayersStorage();
            var transformStorage = clientMatchSimulation.GetITransformStorage();
            var healthPointsStorage = clientMatchSimulation.GetIHealthPointsStorage();
            var maxHealthPointsMessagePackStorage = clientMatchSimulation.GetIMaxHealthPointsMessagePackStorage();

            
            var messageWrapperHandler = new MessageWrapperHandler(udpSendUtils, matchModel.MatchId, transformStorage, playersStorage, healthPointsStorage, maxHealthPointsMessagePackStorage);
            ByteArrayHandler byteArrayHandler = new ByteArrayHandler(messageWrapperHandler);
            
            udpManager.StartListening(byteArrayHandler);
        }

        private void Update()
        {
            clientMatchSimulation?.Tick();
        }

        private void OnDestroy()
        {
            clientMatchSimulation?.StopSystems();
        }
    }
}