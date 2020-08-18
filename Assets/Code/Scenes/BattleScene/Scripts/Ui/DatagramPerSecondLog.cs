using Code.Common.NetworkStatistics;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.Scripts
{
    public class DatagramPerSecondLog : MonoBehaviour
    {
        private Text text;
    
        private void Awake()
        {
            text = GetComponent<Text>();
        }
    
        private void Update()
        {
            int value = NetworkStatisticsStorage.Instance.GetLastFramerate();
            text.text = $"{value} pps";
        }
    }
}
