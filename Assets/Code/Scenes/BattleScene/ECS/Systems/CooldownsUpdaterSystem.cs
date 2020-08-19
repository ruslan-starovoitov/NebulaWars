using Entitas;
using Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common;
using Code.Scenes.BattleScene.Experimental;
using Code.Scenes.BattleScene.Experimental.Approximation;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems
{
    public class CooldownsUpdaterSystem : IExecuteSystem, ITearDownSystem
    {
        private static volatile WeaponInfo[] weaponInfos;
        private static volatile float[] cooldowns;
        private static volatile bool infosWasChanged;
        private static volatile bool cooldownsWasChanged;
        private readonly CannonCooldownsController _cannonCooldownsController;
        private readonly IApproximator<float> approximator;
        private readonly ILog log = LogManager.CreateLogger(typeof(CooldownsUpdaterSystem));

        public CooldownsUpdaterSystem(CannonCooldownsController cannonCooldownsController, IApproximator<float> cannonCooldownApproximator)
        {
            if(cannonCooldownsController == null)
                throw new Exception($"{nameof(CooldownsUpdaterSystem)} {nameof(cannonCooldownsController)} was null");
            
            approximator = cannonCooldownApproximator;

            _cannonCooldownsController = cannonCooldownsController;
            weaponInfos = new WeaponInfo[0];
            cooldowns = new float[0];
        }
            
        public static void SetWeaponsInfos(WeaponInfo[] weaponInfosArg)
        {
            weaponInfos = weaponInfosArg;
            infosWasChanged = true;
        }
        
        public static void SetCooldowns(float[] cooldownsArg)
        {
            cooldowns = cooldownsArg;
            cooldownsWasChanged = true;
        }

        public void Execute()
        {
            if (infosWasChanged)
            {
                _cannonCooldownsController.Load(weaponInfos);
                var dict = new Dictionary<ushort, float>(weaponInfos.Length);
                for (ushort i = 0; i < weaponInfos.Length; i++)
                {
                    dict.Add(i, 0f);
                }
                approximator.Set(dict, Time.time);
                infosWasChanged = false;
            }

            _cannonCooldownsController.SetCooldowns(approximator.Get(Time.time).Values.ToArray());

            if (cooldownsWasChanged)
            {
                var dict = new Dictionary<ushort, float>(cooldowns.Length);
                for (ushort i = 0; i < cooldowns.Length; i++)
                {
                    dict.Add(i, cooldowns[i]);
                }
                approximator.Set(dict, Time.time);
                cooldownsWasChanged = false;
            }
        }

        public void TearDown()
        {
            approximator.Clear();
        }
    }
}