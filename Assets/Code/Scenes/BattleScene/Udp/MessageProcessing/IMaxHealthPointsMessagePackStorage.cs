using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Health;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing
{
    public interface IMaxHealthPointsMessagePackStorage
    {
        void SetNewMaxHealthPoints(MaxHealthPointsMessagePack message);
    }
}