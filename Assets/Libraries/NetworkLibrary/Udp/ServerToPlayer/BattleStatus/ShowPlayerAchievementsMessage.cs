﻿﻿﻿﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus
{
    [ZeroFormattable]
    public struct ShowPlayerAchievementsMessage:ITypedMessage
    {
        [Index(0)] public int MatchId;

        public ShowPlayerAchievementsMessage(int matchId)
        {
            MatchId = matchId;
        }

        public MessageType GetMessageType()
        {
            return MessageType.ShowPlayerAchievements;
        }
    }
}