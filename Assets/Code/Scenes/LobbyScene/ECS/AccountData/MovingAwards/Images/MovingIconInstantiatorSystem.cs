using System;
using System.Collections.Generic;
using Entitas;
using Plugins.submodules.EntitasCore.Entitas.Components;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.Images
{
    /// <summary>
    /// Создаёт GameObject-ы для двигающихся наград из префабов в зависимости от типа награды.
    /// </summary>
    public class MovingIconInstantiatorSystem:ReactiveSystem<LobbyUiEntity>
    {
        private GameObject trophyGameObjectCache;
        private GameObject lootboxPointGameObjectCache;
        private GameObject regularCurrencyGameObjectCache;
        private GameObject premiumCurrencyGameObjectCache;
        private readonly RectTransform movingAwardsParentRectTransform;
        
        public MovingIconInstantiatorSystem(Contexts contexts,
            RectTransform movingAwardsParentRectTransform) 
            : base(contexts.lobbyUi)
        {
            this.movingAwardsParentRectTransform = movingAwardsParentRectTransform;
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.AllOf(LobbyUiMatcher.MovingIcon)
                .NoneOf(LobbyUiMatcher.View, LobbyUiMatcher.Image));
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasMovingIcon && !entity.hasView && !entity.hasImage;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            int index = 0;
            foreach (var entity in entities)
            {
                GameObject awardGo = SpawnAwardPrefab(entity.movingIcon.awardTypeEnum);
                awardGo.name += (index++).ToString();
                entity.AddView(awardGo);
                entity.AddImage(awardGo.GetComponent<Image>());    
            }
        }

        private GameObject SpawnAwardPrefab(AwardTypeEnum awardTypeEnum)
        {
            GameObject prefab;
            switch (awardTypeEnum)
            {
                case AwardTypeEnum.SoftCurrency:
                    prefab = GetRegularCurrencyPrefab();
                    break;
                case AwardTypeEnum.AccountRating:
                    prefab = GetTrophyPrefab();
                    break;
                case AwardTypeEnum.HardCurrency:
                    prefab = GetPremiumCurrencyPrefab();
                    break;
                case AwardTypeEnum.LootboxPoints:
                    prefab = GetLootboxPointPrefab();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(awardTypeEnum), awardTypeEnum, null);
            }
            GameObject result = Object.Instantiate(prefab, movingAwardsParentRectTransform,false);
            return result;
        }
        
        private GameObject GetRegularCurrencyPrefab()
        {
            if (regularCurrencyGameObjectCache == null)
            {
                regularCurrencyGameObjectCache = Resources.Load<GameObject>("Prefabs/RegularCurrencyIcon");
                if (regularCurrencyGameObjectCache == null)
                {
                    throw new Exception("RegularCurrencyIcon prefab not found");
                }    
            }

            return regularCurrencyGameObjectCache;
        }
        
        private GameObject GetPremiumCurrencyPrefab()
        {
            if (premiumCurrencyGameObjectCache == null)
            {
                premiumCurrencyGameObjectCache = Resources.Load<GameObject>("Prefabs/HardCurrencyIcon");
                if (premiumCurrencyGameObjectCache == null)
                {
                    throw new Exception("HardCurrency prefab not found");
                }    
            }

            return premiumCurrencyGameObjectCache;
        }

        private GameObject GetLootboxPointPrefab()
        {
            if (lootboxPointGameObjectCache == null)
            {
                lootboxPointGameObjectCache = Resources.Load<GameObject>("Prefabs/SmallLootboxPoint");
                if (lootboxPointGameObjectCache == null)
                {
                    throw new Exception("SmallLootboxPoint prefab not found");
                }    
            }

            return lootboxPointGameObjectCache;
        }
        
        private GameObject GetTrophyPrefab()
        {
            if (trophyGameObjectCache == null)
            {
                trophyGameObjectCache = Resources.Load<GameObject>("Prefabs/Trophy");
                if (trophyGameObjectCache == null)
                {
                    throw new Exception("AccountRating prefab not found");
                }    
            }

            return trophyGameObjectCache;
        }
    }
}