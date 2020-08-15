﻿﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus
{
    [ZeroFormattable]
    public struct FrameRateMessage : ITypedMessage
    {
        [Index(0)] public readonly float DeltaTime;

        public FrameRateMessage(float deltaTime)
        {
            DeltaTime = deltaTime;
        }

        public MessageType GetMessageType()
        {
            return MessageType.FrameRate;
        }
    }

    public static class ServerTimeConstants
    {
        public const float MaxFps = 10;
        public const float MinDeltaTime = 1f / MaxFps;
    }
}