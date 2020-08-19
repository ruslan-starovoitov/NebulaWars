using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.Listeners
{
    public class ButtonBackListener:MonoBehaviour
    {
        private LobbyEcsController lobbyEcsController;
        private readonly ILog log = LogManager.CreateLogger(typeof(ButtonBackListener));
        
        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                log.Info("Нажата клавиша Escape");
                lobbyEcsController.BackButton_OnClick();
            }
        }

        public void ButtonBack_OnClick()
        {
            lobbyEcsController.BackButton_OnClick();
        }
    }
}