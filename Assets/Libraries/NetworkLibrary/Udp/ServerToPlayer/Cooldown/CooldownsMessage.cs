﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus
{
    [ZeroFormattable]
    public struct CooldownsMessage : ITypedMessage
    {
        [Index(0)] public readonly float AbilityCooldown;
        [Index(1)] public readonly float[] WeaponsCooldowns;

        public CooldownsMessage(float ability, float[] weapons)
        {
            AbilityCooldown = ability;
            WeaponsCooldowns = weapons;
        }
        
        public MessageType GetMessageType()
        {
            return MessageType.Cooldowns;
        }
    }
}