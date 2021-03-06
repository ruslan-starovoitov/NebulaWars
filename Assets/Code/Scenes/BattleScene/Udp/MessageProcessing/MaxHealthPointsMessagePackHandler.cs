﻿using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Health;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing
{
    public class MaxHealthPointsMessagePackHandler : MessageHandler<MaxHealthPointsMessagePack>
    {
        private readonly IMaxHealthPointsMessagePackStorage maxHealthPointsMessagePackStorage;

        public MaxHealthPointsMessagePackHandler(IMaxHealthPointsMessagePackStorage maxHealthPointsMessagePackStorage)
        {
            this.maxHealthPointsMessagePackStorage = maxHealthPointsMessagePackStorage;
        }
        
        protected override void Handle(in MaxHealthPointsMessagePack message, uint messageId, bool needResponse)
        {
            maxHealthPointsMessagePackStorage.SetNewMaxHealthPoints(message);
        }
    }
}