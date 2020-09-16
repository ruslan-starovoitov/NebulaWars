using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.ECS.Systems
{
    public interface IKillMessageStorage
    {
        void AddKillModel(KillMessage message);
    }
}