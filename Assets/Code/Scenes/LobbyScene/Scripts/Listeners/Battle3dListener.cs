using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scenes.LobbyScene.Scripts.Listeners
{
    public class Battle3dListener : MonoBehaviour
    {
        private LobbyEcsController lobbyEcsController;

        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
        }

        public void Open3dBattleScene()
        {
            Destroy(lobbyEcsController);
            SceneManager.LoadScene("Scenes/3dBattle");
        }
    }
}