using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.Scripts.DebugMenu
{
    public class DebugMenuUiStorage : MonoBehaviour
    {
        public GameObject debugMenuRoot;
        public Dropdown qualityDropdown;
        public Slider resolutionSlider;
        public Dropdown resolutionDropdown;
        public Button resolutionApplyButton;
    }
}