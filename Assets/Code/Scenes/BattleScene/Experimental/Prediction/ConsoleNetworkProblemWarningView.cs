using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public class ConsoleNetworkProblemWarningView : INetworkProblemWarningView
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(ConsoleNetworkProblemWarningView));
        public void ShowWarning()
        {
            log.Error("Проблема с сетью");
        }
    }
}