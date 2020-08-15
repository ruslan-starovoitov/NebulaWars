using Libraries.NetworkLibrary.Udp.ServerToPlayer;

namespace Code.Scenes.BattleScene.Scripts
{
    public interface IHealthPointsStorage
    {
        void SetNewHealthPoints(HealthPointsMessagePack message);
    }
}