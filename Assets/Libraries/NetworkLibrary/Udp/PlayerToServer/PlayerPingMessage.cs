﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using ZeroFormatter;
           
namespace NetworkLibrary.NetworkLibrary.Udp.PlayerToServer.Ping
{
    [ZeroFormattable]
    public struct PlayerPingMessage : ITypedMessage
    {
        [Index(0)] public ushort TemporaryId { get; }
        [Index(1)] public int MatchId { get; }

        public PlayerPingMessage(ushort temporaryId, int matchId)
        {
            TemporaryId = temporaryId;
            MatchId = matchId;
        }

        public MessageType GetMessageType() => MessageType.PlayerPing;
    }
}