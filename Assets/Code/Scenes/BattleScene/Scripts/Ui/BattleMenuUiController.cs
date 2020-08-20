using System.Collections;
using Code.Common;
using Code.Scenes.BattleScene.Udp.Experimental;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts.Ui
{
    [RequireComponent(typeof(LobbyLoaderController))]
    [RequireComponent(typeof(BattleUiController))]
    public class BattleMenuUiController : MonoBehaviour
    {
        // private UdpSendUtils udpSendUtils;
        private UiSoundsManager uiSoundsManager;
        private JoysticksManager joysticksManager;
        // private BattleUiController battleUiController;
        private LobbyLoaderController lobbyLoaderController;
        [SerializeField] private VariableJoystick movementJoystick;
        [SerializeField] private VariableJoystick attackJoystick;

        private void Awake()
        {
            lobbyLoaderController = GetComponent<LobbyLoaderController>();
            // battleUiController = GetComponent<BattleUiController>();

            uiSoundsManager = UiSoundsManager.Instance();
            joysticksManager = JoysticksManager.Instance();
        }

        private void Start()
        {
            // udpSendUtils = GetComponent<UdpManager>().GetUdpSendUtils();
            StartCoroutine(LateStart());
        }

        private IEnumerator LateStart()
        {
            yield return null;

            movementJoystick.SetMode(joysticksManager.MovementType);
            attackJoystick.SetMode(joysticksManager.AttackType);
        }

        public void Button_Exit_Click()
        {
            // new StubBattleExitHelper().StubNotifyGameServerAsync(udpSendUtils).ConfigureAwait(false);
            uiSoundsManager.PlayStop();
            lobbyLoaderController.LoadLobbyScene();
        }

        public void Button_Lock_Movement_Click()
        {
            movementJoystick.SetMode(joysticksManager.MovementType);
        }

        public void Button_Lock_Attack_Click()
        {
            attackJoystick.SetMode(joysticksManager.AttackType);
        }
    }
}