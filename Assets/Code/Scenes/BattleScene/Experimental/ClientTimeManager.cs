
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.Experimental
{
    public static class ClientTimeManager
    {
        public const float DelayFramesCount = 3f;
        public const float TimeDelay = ServerTimeConstants.MinDeltaTime * DelayFramesCount;
    }
}