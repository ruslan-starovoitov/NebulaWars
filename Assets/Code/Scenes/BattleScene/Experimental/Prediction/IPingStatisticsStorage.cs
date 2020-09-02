namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public interface IPingStatisticsStorage
    {
        void TrySendPing();
        float GetLastPingSec();
        void PingAnswer(int messagePingMessageId);
    }
}