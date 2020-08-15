using Libraries.NetworkLibrary.Udp.ServerToPlayer;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing
{
    public interface IMaxHealthPointsMessagePackStorage
    {
        void SetNewMaxHealthPoints(MaxHealthPointsMessagePack message);
    }
}