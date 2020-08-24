using System;
using System.Collections;
using Code.Scenes.LobbyScene.ECS;
using Plugins.submodules.EntitasCore.Entitas.Components;
using UnityEngine;
using UnityEngine.Assertions;

namespace Code.Scenes.LobbyScene.Scripts.UiStorages
{
    /// <summary>
    /// Синглтон
    /// Хранит публичные ссылки на ui элементы, которые нужны для рисования анимации начисления наград.
    /// </summary>
    public class MovingAwardsUiElementsStorage : MonoBehaviour
    {
        [Header("Родительские объекты на Canvas")]
        public RectTransform movingAwardImageParentRectTransform;
        public RectTransform movingAwardTextParentRectTransform;
        public RectTransform movingAwardImageUpperObject;
        
        [Header("Старт и финиш для обычной валюты")]
        [SerializeField] private RectTransform softCurrencyStartPoint;
        [SerializeField] private RectTransform softCurrencyFinishPoint;
        
        [Header("Старт и финиш для премиум валюты")]
        [SerializeField] private RectTransform hardCurrencyStartPoint;
        [SerializeField] private RectTransform hardCurrencyFinishPoint;
        
        [Header("Старт и финиш для рейтинга")]
        [SerializeField] private RectTransform trophyStartPoint;
        [SerializeField] private RectTransform trophyFinishPoint;
        
        [Header("Старт и финиш для баллов маленького сундука")]
        [SerializeField] private RectTransform smallLootboxStartPoint;
        [SerializeField] private RectTransform smallLootboxFinishPoint;

        private Vector3 softCurrencyFinishPosition;
        private Vector3 hardCurrencyFinishPosition;
        
        private static MovingAwardsUiElementsStorage instance;
        private void Awake()
        {
            instance = this;
        }

        private IEnumerator Start()
        {
            yield return null;
            softCurrencyFinishPosition = softCurrencyFinishPoint.position;
            hardCurrencyFinishPosition = hardCurrencyFinishPoint.position;
        }

        public static MovingAwardsUiElementsStorage Instance()
        {
            return instance;
        }
        
        public Vector3 GetStartPoint(AwardTypeEnum awardTypeEnum)
        {
            switch (awardTypeEnum)
            {
                case AwardTypeEnum.SoftCurrency:
                    return softCurrencyStartPoint.position;
                case AwardTypeEnum.AccountRating:
                    return trophyStartPoint.position;
                case AwardTypeEnum.HardCurrency:
                    return hardCurrencyStartPoint.position;
                case AwardTypeEnum.LootboxPoints:
                    return smallLootboxStartPoint.position;
                default:
                    throw new ArgumentOutOfRangeException(nameof(awardTypeEnum), awardTypeEnum, null);
            }
        }
        
        public Vector3 GetFinishPoint(AwardTypeEnum awardTypeEnum)
        {
            switch (awardTypeEnum)
            {
                case AwardTypeEnum.SoftCurrency:
                    return softCurrencyFinishPosition;
                case AwardTypeEnum.AccountRating:
                    return trophyFinishPoint.position;
                case AwardTypeEnum.HardCurrency:
                    return hardCurrencyFinishPosition;
                case AwardTypeEnum.LootboxPoints:
                    return smallLootboxFinishPoint.position;
                default:
                    throw new ArgumentOutOfRangeException(nameof(awardTypeEnum), awardTypeEnum, null);
            }
        }

        public void Check()
        {
            Assert.IsNotNull(movingAwardImageParentRectTransform);
            Assert.IsNotNull(movingAwardTextParentRectTransform);
            Assert.IsNotNull(movingAwardImageUpperObject);
            Assert.IsNotNull(softCurrencyStartPoint);
            Assert.IsNotNull(softCurrencyFinishPoint);
            Assert.IsNotNull(hardCurrencyStartPoint);
            Assert.IsNotNull(hardCurrencyFinishPoint);
            Assert.IsNotNull(trophyStartPoint);
            Assert.IsNotNull(trophyFinishPoint);
            Assert.IsNotNull(smallLootboxStartPoint);
            Assert.IsNotNull(smallLootboxFinishPoint);
        }
    }
}