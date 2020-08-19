using Code.Common;
using Code.Scenes.BattleScene.ECS;
using Plugins.submodules.SharedCode.Logger;
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
            Destroy(FindObjectOfType<ClientMatchSimulation>());
            SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }
    }
}

