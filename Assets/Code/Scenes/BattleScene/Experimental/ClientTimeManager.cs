﻿using Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.Experimental
{
    public static class ClientTimeManager
    {
        public const float DelayFramesCount = 2f;
        public const float TimeDelay = ServerTimeConstants.MinDeltaTime * DelayFramesCount;
    }
}