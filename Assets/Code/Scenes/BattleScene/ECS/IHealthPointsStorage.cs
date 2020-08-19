using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Health;

namespace Code.Scenes.BattleScene.ECS
{
    public interface IHealthPointsStorage
    {
        void SetNewHealthPoints(HealthPointsMessagePack message);
    }
}