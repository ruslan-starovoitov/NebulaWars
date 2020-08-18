﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.PlayerToServer
{
    [ZeroFormattable]
    public struct BattleExitMessage:ITypedMessage
    {
        [Index(0)] public int MatchId { get; }
        [Index(1)] public ushort TemporaryId { get; }

        public BattleExitMessage(int matchId, ushort temporaryId)
        {
            MatchId = matchId;
            TemporaryId = temporaryId;
        }

        public MessageType GetMessageType() => MessageType.PlayerExit;
    }
}