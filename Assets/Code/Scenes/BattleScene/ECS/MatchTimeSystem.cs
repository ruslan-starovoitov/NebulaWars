using System;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;

namespace Code.Scenes.BattleScene.ECS
{
    public class MatchTimeSystem : IExecuteSystem, IMatchTimeStorage
    {
        private float matchTime;
        private readonly INetworkTimeManager networkTimeManager;

        public MatchTimeSystem(INetworkTimeManager networkTimeManager)
        {
            this.networkTimeManager = networkTimeManager;
        }
        
        public void Execute()
        {
            if (!networkTimeManager.IsReady())
            {
                throw new Exception("Не установлено время старта матча");
            }
            
            matchTime = networkTimeManager.GetMatchTime();
        }

        public float GetMatchTime()
        {
            return matchTime;
        }
    }
}