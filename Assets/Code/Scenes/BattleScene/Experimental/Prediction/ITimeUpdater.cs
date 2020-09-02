namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public interface ITimeUpdater
    {
        void NewSnapshotReceived(int tickNumber, float tickMatchTime);
    }
}