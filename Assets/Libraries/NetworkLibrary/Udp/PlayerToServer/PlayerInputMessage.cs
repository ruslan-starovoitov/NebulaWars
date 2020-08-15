﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;
using ZeroFormatter;

namespace NetworkLibrary.NetworkLibrary.Udp.PlayerToServer.UserInputMessage
{
    [ZeroFormattable]
    public struct PlayerInputMessage : ITypedMessage
    {
        [Index(0)] public ushort TemporaryId { get; }
        [Index(1)] public int MatchId { get; }
        [Index(2)] public float X { get; }
        [Index(3)] public float Y { get; }
        [Index(4)] public float Angle { get; }
        [Index(5)] public bool UseAbility { get; }

        public PlayerInputMessage(ushort temporaryId, int matchId, float x, float y, float angle, bool ability)
        {
            TemporaryId = temporaryId;
            MatchId = matchId;
            X = x;
            Y = y;
            Angle = angle;
            UseAbility = ability;
        }

        public MessageType GetMessageType() => MessageType.PlayerInput;

        public Vector2 GetVector2() => new Vector2(X, Y);
    }
}