﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System.Collections.Generic;
        using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.ServerToPlayer
{
    [ZeroFormattable]
    public struct HealthPointsMessage:ITypedMessage
    {
        [Index(0)] public readonly float Value;
        
        public HealthPointsMessage(float value)
        {
            Value = value;
        }

        public MessageType GetMessageType()
        {
            return MessageType.HealthPoints;
        }
    }
    
    [ZeroFormattable]
    public class HealthPointsMessagePack:ITypedMessage
    {
        [Index(0)] public virtual Dictionary<ushort, float> entityIdToValue { get; set; }

        public HealthPointsMessagePack()
        {
        }
        
        public HealthPointsMessagePack(Dictionary<ushort, float> entityIdToValue)
        {
            this.entityIdToValue = entityIdToValue;
        }

        public MessageType GetMessageType()
        {
            return MessageType.HealthPointsMessagePack;
        }
    }

    [ZeroFormattable]
    public class MaxHealthPointsMessagePack : ITypedMessage
    {
        [Index(0)] public virtual Dictionary<ushort, float> entityIdToValue { get; set; }

        public MaxHealthPointsMessagePack()
        {
        }

        public MaxHealthPointsMessagePack(Dictionary<ushort, float> entityIdToValue)
        {
            this.entityIdToValue = entityIdToValue;
        }

        public MessageType GetMessageType()
        {
            return MessageType.MaxHealthPointsMessagePack;
        }
    }
}