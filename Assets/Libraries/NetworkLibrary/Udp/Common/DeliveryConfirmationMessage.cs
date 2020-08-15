﻿﻿﻿﻿﻿﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.Common
{
    [ZeroFormattable]
    public struct DeliveryConfirmationMessage:ITypedMessage
    {
        [Index(0)] public uint MessageNumberThatConfirms;
        [Index(1)] public ushort PlayerId;
        [Index(2)] public int MatchId;
        
        public MessageType GetMessageType() => MessageType.DeliveryConfirmation;

        public DeliveryConfirmationMessage(uint messageNumberThatConfirms, ushort playerId, int matchId)
        {
            MessageNumberThatConfirms = messageNumberThatConfirms;
            PlayerId = playerId;
            MatchId = matchId;
        }
    }
}