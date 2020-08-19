using System.Collections.Generic;
using Code.Scenes.BattleScene.Experimental.Approximation;
using Entitas;
using Libraries.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public class TimeSpeedSystem : IExecuteSystem, ITearDownSystem
    {
        private static readonly object LockObj = new object();
        private static float _serverDeltaTime;
        private static uint _lastMessageId;
        private static bool WasChanged;
        private readonly IApproximator<float> approximator;
        private readonly float defaultFixedDeltaTime;
        private readonly GameContext gameContext;
        private readonly ILog log = LogManager.CreateLogger(typeof(TimeSpeedSystem));

        public TimeSpeedSystem(Contexts contexts, IApproximator<float> timeApproximator)
        {
            gameContext = contexts.game;
            _lastMessageId = 0;
            WasChanged = false;
            approximator = timeApproximator;
            _serverDeltaTime = ServerTimeConstants.MinDeltaTime;
            defaultFixedDeltaTime = Time.fixedDeltaTime;

            approximator.Set(new Dictionary<ushort, float>(1) { { 0, ServerTimeConstants.MinDeltaTime } }, Time.time - Time.deltaTime);
            approximator.Set(new Dictionary<ushort, float>(1) { { 0, ServerTimeConstants.MinDeltaTime } }, Time.time);
        }

        public static void SetFrameRate(uint messageId, float serverDeltaTime)
        {
            lock (LockObj)
            {
                if (messageId > _lastMessageId)
                {
                    _lastMessageId = messageId;
                    _serverDeltaTime = serverDeltaTime;
                    WasChanged = true;
                }
                else if(serverDeltaTime > _serverDeltaTime)
                {
                    _serverDeltaTime = (_serverDeltaTime + serverDeltaTime) * 0.5f;
                    WasChanged = true;
                }
            }
        }

        public void TearDown()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
        }

        public void Execute()
        {
            var dict = approximator.Get(Time.time);
            var serverDeltaTime = dict[0];

            lock (LockObj)
            {
                if (WasChanged)
                {
                    //log.Debug("Server FPS: " + 1f / _serverDeltaTime);
                    approximator.Set(new Dictionary<ushort, float>(1) { { 0, _serverDeltaTime } }, Time.time);
                    WasChanged = false;
                }
            }

            var timeScale = ServerTimeConstants.MinDeltaTime / serverDeltaTime;
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime * timeScale;
        }
    }
}