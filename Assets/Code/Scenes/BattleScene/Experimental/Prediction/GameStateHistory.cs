using Plugins.submodules.SharedCode.LagCompensation;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    public interface IGameStateHistory
    {
        FullSnapshot Get(int serverTickNumber);
    }
}