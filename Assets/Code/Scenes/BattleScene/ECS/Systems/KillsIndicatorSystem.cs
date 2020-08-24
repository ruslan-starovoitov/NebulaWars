// using Code.Common;
// using Code.Scenes.BattleScene.Experimental;
// using Code.Scenes.BattleScene.Scripts;
// using Entitas;
// using System;
// using System.Collections.Concurrent;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.RegularExpressions;
// using Code.Common.Storages;
// using Plugins.submodules.SharedCode.Logger;
// using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Code.Scenes.BattleScene.ECS.Systems
// {
//     public class KillsIndicatorSystem : IExecuteSystem, ITearDownSystem
//     {
//         private static volatile bool wasChanged;
//         private static readonly ConcurrentBag<KillMessage> Messages = new ConcurrentBag<KillMessage>();
//         private static readonly Color endColor = Color.red;
//         private readonly ILog log = LogManager.CreateLogger(typeof(KillsIndicatorSystem));
//         private readonly KillInfoObject messagePrototype;
//         private readonly Transform messagesContainer;
//         private readonly Text kills;
//         private readonly Text alive;
//         private readonly Queue<KillInfoObject> messageObjects;
//         private readonly int currentPlayerId;
//         private readonly int baseKillsFontSize;
//         private readonly int baseAliveFontSize;
//         private readonly Color baseKillsColor;
//         private readonly Color baseAliveColor;
//         private float killsChangingPercentage;
//         private float aliveChangingPercentage;
//         private bool killsIsChanging;
//         private bool aliveIsChanging;
//         private int playerKillsCount;
//         private int aliveCount;
//         private const int MaxMessagesCount = 5;
//         private const float OnNewMessageFading = 1f / MaxMessagesCount;
//         private const float MaxFadingTime = 30f;
//         private const float PerSecondFading = 1f / MaxFadingTime;
//         private const float changingTime = 0.75f;
//         private const float minFontScaling = 0.75f;
//         private const float maxFontScaling = 1.25f;
//         private const float deltaFontScaling = maxFontScaling - minFontScaling;
//         private const float changingStep = 1f / changingTime;
//
//         public KillsIndicatorSystem(KillInfoObject killMessage, Transform container, Text killsText, Text aliveText, int aliveCount)
//         {
//             if (killMessage == null)
//                 throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(killMessage)} was null");
//             if (container == null)
//                 throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(container)} was null");
//             if (killsText == null)
//                 throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(killsText)} was null");
//             if (aliveText == null)
//                 throw new Exception($"{nameof(KillsIndicatorSystem)} {nameof(aliveText)} was null");
//
//             messagePrototype = killMessage;
//             messagesContainer = container;
//             kills = killsText;
//             alive = aliveText;
//             baseKillsFontSize = kills.fontSize;
//             baseAliveFontSize = alive.fontSize;
//             baseKillsColor = kills.color;
//             baseAliveColor = alive.color;
//             killsChangingPercentage = 0f;
//             aliveChangingPercentage = 0f;
//
//             messageObjects = new Queue<KillInfoObject>(MaxMessagesCount);
//             currentPlayerId = PlayerIdStorage.AccountId;
//             this.aliveCount = aliveCount;
//             alive.text = aliveCount.ToString("D2");
//         }
//             
//         public static void AddNewKillInfo(KillMessage message)
//         {
//             Messages.Add(message);
//             wasChanged = true;
//         }
//
//         private static string GetName(int playerId) =>
//             MatchModelStorage.Instance.GetMatchModel()
//                 .PlayerModels.FirstOrDefault(player => player.AccountId == playerId)
//                 .Nickname;
//
//         private static string GetName(ViewTypeEnum typeId)
//         {
//             return Regex.Replace(typeId.ToString("G"), @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
//         }
//
//         private static void AnimateText(Text text, int baseFontSize, Color baseColor, ref bool isChanging, ref float percentage)
//         {
//             if (!isChanging) return;
//             percentage += Time.deltaTime * changingStep;
//             if (percentage <= 1f)
//             {
//                 text.fontSize = Mathf.RoundToInt(baseFontSize * (minFontScaling + percentage * deltaFontScaling));
//                 text.color = Color.Lerp(baseColor, endColor, percentage);
//             }
//             else
//             {
//                 text.fontSize = baseFontSize;
//                 text.color = baseColor;
//                 percentage = 0f;
//                 isChanging = false;
//             }
//         }
//         
//         public void Execute()
//         {
//             AnimateText(kills, baseKillsFontSize, baseKillsColor, ref killsIsChanging, ref killsChangingPercentage);
//             AnimateText(alive, baseAliveFontSize, baseAliveColor, ref aliveIsChanging, ref aliveChangingPercentage);
//
//             if (wasChanged)
//             {
//                 while (Messages.TryTake(out var message))
//                 {
//                     var newMessage = UnityEngine.Object.Instantiate(messagePrototype, messagesContainer);
//
//                     var killerName = GetName(message.KillerId) ?? GetName(message.KillerType);
//                     var victimName = GetName(message.VictimId) ?? GetName(message.VictimType);
//
//                     newMessage.SetKillerName(killerName);
//                     newMessage.SetKillerSprite(PreviewsManager.GetSprite(message.KillerType));
//                     newMessage.SetVictimName(victimName);
//                     newMessage.SetVictimSprite(PreviewsManager.GetSprite(message.VictimType));
//
//                     foreach (var killInfoObject in messageObjects)
//                     {
//                         var transform = killInfoObject.transform;
//                         var position = transform.localPosition;
//                         position.y -= 50f;
//                         transform.localPosition = position;
//                         killInfoObject.DecreaseTransparency(OnNewMessageFading);
//                     }
//
//                     messageObjects.Enqueue(newMessage);
//                     if (message.KillerId == currentPlayerId)
//                     {
//                         playerKillsCount++;
//                         killsChangingPercentage = 0f;
//                         killsIsChanging = true;
//                     }
//                     aliveCount--;
//                     aliveChangingPercentage = 0f;
//                     aliveIsChanging = true;
//                 }
//
//                 kills.text = playerKillsCount.ToString("D2");
//                 alive.text = aliveCount.ToString("D2");
//                 wasChanged = false;
//             }
//
//             var delta = Time.deltaTime * PerSecondFading;
//
//             foreach (var killInfoObject in messageObjects)
//             {
//                 killInfoObject.DecreaseTransparency(delta);
//             }
//             
//             while (messageObjects.Count > 0 && messageObjects.Peek().currentTransparency <= 0)
//             {
//                 UnityEngine.Object.Destroy(messageObjects.Dequeue().gameObject);
//             }
//         }
//
//         public void TearDown()
//         {
//             while (!Messages.IsEmpty)
//             {
//                 Messages.TryTake(out _);
//             }
//         }
//     }
// }