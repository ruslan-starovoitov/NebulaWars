﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.ServerToPlayer
{
    [ZeroFormattable]
    public struct MaxShieldPointsMessage : ITypedMessage
    {
        [Index(0)] public readonly float Value;
        
        public MaxShieldPointsMessage(float value)
        {
            Value = value;
        }

        public MessageType GetMessageType()
        {
            return MessageType.MaxShieldPoints;
        }
    }
}