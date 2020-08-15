using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts
{
    public class TargetFrameRate : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
        }
    }
}