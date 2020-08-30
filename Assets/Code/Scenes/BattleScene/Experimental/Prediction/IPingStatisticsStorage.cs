namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public interface IPingStatisticsStorage
    {
        void TrySendPing();
        float GetLastPingMs();
        void PingAnswer(int messagePingMessageId);
    }
}