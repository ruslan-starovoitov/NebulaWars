﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System.Collections.Generic;
            using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using ZeroFormatter;

namespace NetworkLibrary.NetworkLibrary.Udp.PlayerToServer.UserInputMessage
{
    // [ZeroFormattable]
    // public struct PlayerInputMessage : ITypedMessage
    // {
    //     [Index(0)] public ushort TemporaryId { get; }
    //     [Index(1)] public int MatchId { get; }
    //     [Index(2)] public float X { get; }
    //     [Index(3)] public float Y { get; }
    //     [Index(4)] public float Angle { get; }
    //     [Index(5)] public bool UseAbility { get; }
    //
    //     public PlayerInputMessage(ushort temporaryId, int matchId, float x, float y, float angle, bool ability)
    //     {
    //         TemporaryId = temporaryId;
    //         MatchId = matchId;
    //         X = x;
    //         Y = y;
    //         Angle = angle;
    //         UseAbility = ability;
    //     }
    //
    //     public MessageType GetMessageType() => MessageType.PlayerInputMessagesPack;
    //
    //     public Vector2 GetVector2() => new Vector2(X, Y);
    // }
    
    [ZeroFormattable]
    public class InputMessagesPack : ITypedMessage
    {
        [Index(0)] public virtual ushort TemporaryId { get; set; }
        [Index(1)] public virtual int MatchId { get; set; }
        //inputId + data
        [Index(2)] public virtual Dictionary<int, InputMessageModel> History { get; set; }

        public MessageType GetMessageType() => MessageType.PlayerInputMessagesPack;
    }

    [ZeroFormattable]
    public class InputMessageModel: ITypedMessage
    {
        [Index(0)] public virtual float X { get; set; }
        [Index(1)] public virtual float Y { get; set; }
        [Index(2)] public virtual float Angle { get; set; }
        [Index(3)] public virtual bool UseAbility { get; set; }
        [Index(4)] public virtual int TickNumber { get; set; }
        
        public MessageType GetMessageType()
        {
            return MessageType.InputMessageModel;
        }
    }

    public static class InputExtension
    {
        public static Vector2 GetVector2(this InputMessageModel model)
        {
            return new Vector2(model.X, model.Y);
        }
    } 
}