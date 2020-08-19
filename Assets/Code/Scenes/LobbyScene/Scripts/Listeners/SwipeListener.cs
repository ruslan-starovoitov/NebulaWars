using System;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scenes.LobbyScene.Scripts.Listeners
{
    /// <summary>
    /// Отвечат за отлавливание горизонтальных свайпов и нажатий на кнокпи листания кораблей.
    /// </summary>
    public class SwipeListener : MonoBehaviour, IBeginDragHandler,IDragHandler
    {
        private LobbyEcsController lobbyEcsController;
        private readonly ILog log = LogManager.CreateLogger(typeof(SwipeListener));
        
        private void Awake()
        {
            lobbyEcsController = FindObjectOfType<LobbyEcsController>()
                ?? throw new NullReferenceException(nameof(LobbyEcsController));
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.delta.x > 0)
            {
                log.Info("Старт сдвигания вправо");
                lobbyEcsController.ShiftWarshipsRight();
            }
            else
            {
                log.Info("Старт сдвигания влево");
                lobbyEcsController.ShiftWarshipsLeft();
            }
        }
        
        public void ShiftWarships_RightButton_OnClick()
        {
            lobbyEcsController.ShiftWarshipsLeft();
        }

        public void ShiftWarships_LeftButton_OnClick()
        {
            lobbyEcsController.ShiftWarshipsRight();
        }

        /// <summary>
        /// Необходим для работы свайпов
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData){}
    }
}
