using System.Collections;
using System.Collections.Generic;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.Scripts.Shop
{
 
    /// <summary>
    /// Передвигает меню по указанным координатам. Нужно после нажатия на кнопки снизу.
    /// </summary>
    public class ScrollViewSmoothMovementBehaviour : MonoBehaviour
    {
        private float screenWidth;
        private ScrollRect scrollRect;
        private RectTransform scrollViewContent;
        private Dictionary<string, float> sectionStartPosition;
        private Dictionary<SectionTypeEnum, string> sectionNames;
        private readonly ILog log = LogManager.CreateLogger(typeof(ScrollViewSmoothMovementBehaviour));

        private void Awake()
        {
            ShopUiStorage shopUiStorage = FindObjectOfType<ShopUiStorage>();
            scrollViewContent = shopUiStorage.shopScrollViewContent;
            screenWidth = shopUiStorage.canvasCameraSpace.rect.width;
            scrollRect = shopUiStorage.scrollRect;
        }

        /// <summary>
        /// Нужно для того, чтобы можно было показать раздел с валютой, которой не хватает
        /// </summary>
        /// <param name="sectionNamesArg"></param>
        public void InitRequiredSections(Dictionary<SectionTypeEnum, string> sectionNamesArg)
        {
            sectionNames = sectionNamesArg;
        }
        
        /// <summary>
        /// Нужно для того, чтобы можно было показать любой раздел в магазине
        /// </summary>
        /// <param name="sectionStartPositionArg"></param>
        public void Initialize(Dictionary<string, float> sectionStartPositionArg)
        {
            sectionStartPosition = sectionStartPositionArg;
        }

        /// <summary>
        /// Вызов перелистывания scroll view к разделу с недостающей валютой
        /// </summary>
        /// <param name="sectionType"></param>
        public void StartMovement(SectionTypeEnum sectionType)
        {
            if (sectionNames.TryGetValue(sectionType, out string sectionName))
            {
                StartMovement(sectionName);
            }
            else
            {
                log.Error($"Не удалось начать перелистывание к секции sectionType {sectionType}. " +
                          $"Возможно секция не была инициализирована");
            }
        }

        /// <summary>
        /// Вызов перелистывания к любому разделу
        /// </summary>
        /// <param name="sectionName"></param>
        public void StartMovement(string sectionName)
        {
            if (sectionStartPosition != null)
            {
                if (sectionStartPosition.TryGetValue(sectionName, out float xPosition))
                {
                    StopAllCoroutines();
                    StartCoroutine(Move(xPosition));
                }
                else
                {
                    log.Error($"Не найдена координата раздела с именем {sectionName}");
                }
            }
            else
            {
                log.Error("Позиции разделов не инициализированы.");
            }
        }

        /// <summary>
        /// Перелистывние к левому краю раздела.
        /// </summary>
        /// <param name="xTargetPosition"></param>
        /// <returns></returns>
        private IEnumerator Move(float xTargetPosition)
        {
            DisableElasticity();
            
            
            //начать перемотку к нужному разделу
            if (xTargetPosition + screenWidth > scrollViewContent.rect.width)
            {
                xTargetPosition = scrollViewContent.rect.width - screenWidth;
            }

            const float stepSizeAbs = 50;
            float pathLength;
            do
            {
                float currentPosition = GetCurrentScrollViewPosition();
                pathLength = Mathf.Abs(xTargetPosition - currentPosition);
                float currentStepSize = Mathf.Min(pathLength, stepSizeAbs);
                if (xTargetPosition < currentPosition)
                {
                    currentStepSize *= -1;
                }
                
                float xPosition = currentPosition + currentStepSize;
                var anchoredPosition = scrollViewContent.anchoredPosition;
                var newPosition = new Vector2(-xPosition, anchoredPosition.y);
                scrollViewContent.anchoredPosition = newPosition;
                yield return null;
            } 
            while (pathLength > 0.001);
            
            EnableElasticity();
        }

        private void DisableElasticity()
        {
            scrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        }

        private void EnableElasticity()
        {
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
        }
        private float GetCurrentScrollViewPosition()
        {
            return -scrollViewContent.anchoredPosition.x;
        }
    }
}