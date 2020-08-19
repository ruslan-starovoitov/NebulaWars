using System;
using System.Collections.Generic;
using Code.Common;
using Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation.UiWindow;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.Scripts.Shop.PurchaseConfirmation
{
    public class InsufficientResourceErrorHandler : MonoBehaviour
    {
        private TextTooltip textTooltip;
        private LobbyEcsController lobbyEcsController;
        private ScrollViewSmoothMovementBehaviour scrollViewSmoothMovementBehaviour;
        private readonly ILog log = LogManager.CreateLogger(typeof(InsufficientResourceErrorHandler));

        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>();
            scrollViewSmoothMovementBehaviour = FindObjectOfType<ScrollViewSmoothMovementBehaviour>();
            textTooltip = FindObjectOfType<TextTooltip>();
        }

        public void ShowError(SectionTypeEnum sectionTypeEnum)
        {
            //скрыть меню подтверждения покупки
            lobbyEcsController.ClosePurchaseConfirmationWindow();
            //начать перелистывать на раздел с валютой
            scrollViewSmoothMovementBehaviour.StartMovement(sectionTypeEnum);
            //todo показать всплывающий текст ошибки

            switch (sectionTypeEnum)
            {
                case SectionTypeEnum.SoftCurrency:
                    textTooltip.Show("Not enough coins.");
                    break;
                case SectionTypeEnum.HardCurrency:
                    textTooltip.Show("Not enough gems.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sectionTypeEnum), sectionTypeEnum, null);
            }
        }
    }
}