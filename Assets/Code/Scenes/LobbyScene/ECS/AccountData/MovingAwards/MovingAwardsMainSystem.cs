using System;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.EntitasCore.Entitas.Components;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards
{
    /// <summary>
    /// Создаёт компоненты c информацией про награды и сообщение о том, что нужно показать текст.
    /// </summary>
    public class MovingAwardsMainSystem : IExecuteSystem
    {
        private readonly object lockObj = new object();
        private readonly LobbyUiContext lobbyUiContext;
        private RewardsThatHaveNotBeenShown rewardsThatHaveNotBeenShown;

        public MovingAwardsMainSystem(Contexts contexts)
        {
            lobbyUiContext = contexts.lobbyUi;
        }
        
        public void CreateAwards(RewardsThatHaveNotBeenShown rewardsThatHaveNotBeenShownArgs)
        {
            lock (lockObj)
            {
                rewardsThatHaveNotBeenShown = rewardsThatHaveNotBeenShownArgs;
            }
        }
        
        public void Execute()
        {
            lock (lockObj)
            {
                if (rewardsThatHaveNotBeenShown != null)
                {
                    DateTime startSpawnTime = DateTime.Now;

                    if (rewardsThatHaveNotBeenShown.AccountRatingDelta > 0)
                    {
                        AddAwardSpawnCommand(AwardTypeEnum.AccountRating, rewardsThatHaveNotBeenShown.AccountRatingDelta, startSpawnTime);
                        startSpawnTime += TimeSpan.FromSeconds(2);
                    }

                    if (rewardsThatHaveNotBeenShown.SoftCurrencyDelta > 0)
                    {
                        AddAwardSpawnCommand(AwardTypeEnum.SoftCurrency, rewardsThatHaveNotBeenShown.SoftCurrencyDelta, startSpawnTime);
                        startSpawnTime += TimeSpan.FromSeconds(2);
                    }
                    
                    if (rewardsThatHaveNotBeenShown.HardCurrencyDelta > 0)
                    {
                        AddAwardSpawnCommand(AwardTypeEnum.HardCurrency, rewardsThatHaveNotBeenShown.HardCurrencyDelta, startSpawnTime);
                        startSpawnTime += TimeSpan.FromSeconds(2);
                    }
                    
                    if (rewardsThatHaveNotBeenShown.LootboxPointsDelta > 0)
                    {
                        AddAwardSpawnCommand(AwardTypeEnum.LootboxPoints, rewardsThatHaveNotBeenShown.LootboxPointsDelta, startSpawnTime);
                        startSpawnTime += TimeSpan.FromSeconds(2);
                    }
                    
                    //TODO добавить другие виды наград
                    
                    rewardsThatHaveNotBeenShown = null;
                }
            }
        }

        private void AddAwardSpawnCommand(AwardTypeEnum awardTypeEnum, int quantity, DateTime spawnStartTime)
        {
            LobbyUiEntity entity = lobbyUiContext.CreateEntity();
            entity.AddCommandToCreateAwardImages(quantity, awardTypeEnum , spawnStartTime);
        }
    }
}