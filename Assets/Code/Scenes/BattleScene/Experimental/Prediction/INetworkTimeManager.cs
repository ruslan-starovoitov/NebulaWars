namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public interface INetworkTimeManager
    {
        bool IsReady();
        float GetMatchTime();
    }
}