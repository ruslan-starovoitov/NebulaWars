using System;
using System.Collections.Generic;
using Code.Common.Logger;
using Code.Scenes.BattleScene.Udp.Experimental;
using Entitas;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Code.BattleScene.ECS.Systems
{
    public class PlayerInputSenderSystem : ReactiveSystem<InputEntity>
    {
        private readonly UdpSendUtils udpSendUtils;
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerInputSenderSystem));

        public PlayerInputSenderSystem(Contexts contexts, UdpSendUtils udpSendUtils) : base(contexts.input)
        {
            this.udpSendUtils = udpSendUtils;
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            var matcher = InputMatcher.AnyOf(InputMatcher.Movement, InputMatcher.Attack, InputMatcher.TryingToUseAbility);
            return context.CreateCollector(matcher);
        }

        protected override bool Filter(InputEntity entity)
        {
            return entity.hasMovement || entity.hasAttack || entity.isTryingToUseAbility;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            float x = 0f, y = 0f, angle = float.NaN;
            bool useAbility = false;

            foreach (var inputEntity in entities)
            {
                if (inputEntity.hasMovement)
                {
                    x = inputEntity.movement.x;
                    y = inputEntity.movement.y;
                }

                if (inputEntity.hasAttack)
                {
                    angle = inputEntity.attack.angle;
                }

                useAbility |= inputEntity.isTryingToUseAbility;
            }

            try
            {
                udpSendUtils.SendInputMessage(x, y, angle, useAbility);
            }
            catch (Exception e)
            {
                log.Error("Упало при отправке");
            }
        }
    }
}