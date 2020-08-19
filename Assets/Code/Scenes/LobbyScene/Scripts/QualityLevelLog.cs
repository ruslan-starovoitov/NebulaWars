using System.Linq;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts
{
    public class QualityLevelLog : MonoBehaviour
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(QualityLevelLog));
      
        private void Awake()
        {
            int qualityLevelIndex = QualitySettings.GetQualityLevel();
            string qualityLevelName = QualitySettings.names[qualityLevelIndex];
            log.Info($"qualityLevelName {qualityLevelName} qualityLevelIndex {qualityLevelIndex}");
        }
    }
}