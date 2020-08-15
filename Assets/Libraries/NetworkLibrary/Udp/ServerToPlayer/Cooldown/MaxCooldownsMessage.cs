﻿﻿﻿﻿﻿﻿﻿using NetworkLibrary.NetworkLibrary.Udp;
using ZeroFormatter;

namespace Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus
{
    [ZeroFormattable]
    public class CooldownsInfosMessage : ITypedMessage
    {
        [Index(1)] public virtual WeaponInfo[] WeaponsInfos { get; set; }
        [Index(0)] public virtual float AbilityMaxCooldown { get; set; }
        //[Index(2)] public readonly ViewTypeId AbilityViewType;

        public CooldownsInfosMessage()
        {

        }

        public CooldownsInfosMessage(float abilityCooldown, WeaponInfo[] weaponsInfos)
        {
            WeaponsInfos = weaponsInfos;
            AbilityMaxCooldown = abilityCooldown;
            //AbilityViewType = abilityViewType;
        }
        
        public MessageType GetMessageType()
        {
            return MessageType.CooldownsInfos;
        }
    }

    [ZeroFormattable]
    public struct WeaponInfo
    {
        [Index(0)] public readonly ViewTypeId ViewType;
        [Index(1)] public readonly float Cooldown;

        public WeaponInfo(ViewTypeId viewType, float cooldown)
        {
            ViewType = viewType;
            Cooldown = cooldown;
        }

        public static implicit operator WeaponInfo ((ViewTypeId viewType, float cooldown) tuple)
        {
            var (viewType, cooldown) = tuple;
            return new WeaponInfo(viewType, cooldown);
        }

        public static implicit operator (ViewTypeId viewType, float cooldown) (WeaponInfo weaponInfo)
        {
            return (weaponInfo.ViewType, weaponInfo.Cooldown);
        }
    }
}