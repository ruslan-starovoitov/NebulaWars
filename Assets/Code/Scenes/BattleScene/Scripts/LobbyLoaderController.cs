using Code.Common;
using Code.Common.Logger;
using Code.Scenes.BattleScene.ECS;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Отвечает за загрузку лобби.
    /// </summary>
    public class LobbyLoaderController : MonoBehaviour
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(LobbyLoaderController));

        public void LoadLobbyScene()
        {
            Destroy(FindObjectOfType<MatchSimulation>());
            SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }
    }
}

