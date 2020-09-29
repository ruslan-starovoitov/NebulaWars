using Code.Scenes.BattleScene.Experimental;
using Entitas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Code.Common.Storages;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Scenes.BattleScene.ECS.Systems
{
    //todo это нужно разбить
    /// <summary>
    /// Отвечает за показ сообщений при смерти
    /// </summary>
    public class KillsIndicatorSystem : IExecuteSystem, IKillMessageStorage
    {
        private int aliveCount;
        private readonly Text kills;
        private readonly Text alive;
        private int playerKillsCount;
        private readonly int currentPlayerId;
        private const int MaxMessagesCount = 5;
        private const float MaxFadingTime = 30f;
        private readonly Transform messagesContainer;
        private readonly KillModel messagePrefabPrototype;
        private readonly PlayerNameHelper playerNameHelper;

        private readonly Queue<KillModel> messageObjects;
        private const float PerSecondFading = 1f / MaxFadingTime;
        private const float NewMessageFading = 1f / MaxMessagesCount;
        
        private readonly ILog log = LogManager.CreateLogger(typeof(KillsIndicatorSystem));
        private readonly ConcurrentQueue<KillMessage> messages = new ConcurrentQueue<KillMessage>();
        
        public KillsIndicatorSystem(KillModel killMessagePrefab, Transform container, Text killsText, 
            Text aliveText, int aliveCount, PlayerNameHelper playerNameHelper)
        {
            if (killMessagePrefab == null)
            {
                throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(killMessagePrefab)} was null");
            }
            if (container == null)
            {
                throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(container)} was null");
            }
            if (killsText == null)
            {
                throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(killsText)} was null");
            }
            if (aliveText == null)
            {
                throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(aliveText)} was null");
            }

            messagePrefabPrototype = killMessagePrefab;
            messagesContainer = container;
            kills = killsText;
            alive = aliveText;
            messageObjects = new Queue<KillModel>(MaxMessagesCount);
            currentPlayerId = PlayerIdStorage.AccountId;
            this.aliveCount = aliveCount;
            this.playerNameHelper = playerNameHelper;
            alive.text = aliveCount.ToString("D2");
        }
            
        public void Execute()
        {
            while (messages.TryDequeue(out KillMessage killMessage))
            {
                ShowNewMessage(killMessage);
            }

            alive.text = aliveCount.ToString("D2");
            float delta = Time.deltaTime * PerSecondFading;
            kills.text = playerKillsCount.ToString("D2");
            foreach (var killInfoObject in messageObjects)
            {
                killInfoObject.DecreaseTransparency(delta);
            }
            
            while (messageObjects.Count > 0 && messageObjects.Peek().currentTransparency <= 0)
            {
                Object.Destroy(messageObjects.Dequeue().gameObject);
            }
        }

        private void ShowNewMessage(KillMessage message)
        {
            string killerName = playerNameHelper.GetName(message.KillerId, message.KillerType);
            string victimName = playerNameHelper.GetName(message.VictimId, message.VictimType);
            foreach (var killInfoObject in messageObjects)
            {
                Transform transform = killInfoObject.transform;
                transform.localPosition = transform.localPosition + new Vector3(0,-50f);
                killInfoObject.DecreaseTransparency(NewMessageFading);
            }

            if (message.KillerId == currentPlayerId)
            {
                playerKillsCount++;
            }
            aliveCount--;


            KillModel newMessage = Object.Instantiate(messagePrefabPrototype, messagesContainer);
            newMessage.SetKillerName(killerName);
            Texture2D texture2D = Texture2D.whiteTexture;
            // Sprite sprite = Sprite.Create(texture2D, new Rect(0,0,100,100), Vector2.zero);
            newMessage.SetVictimName(victimName);
                
            // newMessage.SetKillerSprite(sprite);
            // newMessage.SetVictimSprite(sprite);
            //todo установить нормальные спрайты
            // newMessage.SetKillerSprite(PreviewsManager.GetSprite(message.KillerType));
            // newMessage.SetVictimSprite(PreviewsManager.GetSprite(message.VictimType));
                
            messageObjects.Enqueue(newMessage);
        }
        
        public void AddKillModel(KillMessage message)
        {
            messages.Enqueue(message);
            log.Debug($"Сообщение об убийстве {message.KillerId} {message.KillerType} {message.VictimId}" +
                      $" {message.VictimType}");
        }
    }
}