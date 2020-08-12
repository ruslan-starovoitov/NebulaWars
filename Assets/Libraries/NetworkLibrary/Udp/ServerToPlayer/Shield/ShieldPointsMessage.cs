﻿﻿﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.ServerToPlayer
{
    [ZeroFormattable]
    public struct ShieldPointsMessage:ITypedMessage
    {
        [Index(0)] public readonly float Value;
        
        public ShieldPointsMessage(float value)
        {
            Value = value;
        }

        public MessageType GetMessageType()
        {
            return MessageType.ShieldPoints;
        }
    }
}