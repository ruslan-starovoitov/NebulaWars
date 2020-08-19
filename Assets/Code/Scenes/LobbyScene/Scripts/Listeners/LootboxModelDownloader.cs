using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Storages;
using JetBrains.Annotations;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using ZeroFormatter;

namespace Code.Scenes.LobbyScene.Scripts.Listeners
{
    public class LootboxModelDownloader
    {
        private Task<LootboxModel> task;
        private readonly ILog log = LogManager.CreateLogger(typeof(LootboxModelDownloader));
        public static LootboxModelDownloader Instance { get; } = new LootboxModelDownloader();

        public void StartDownloading()
        {
            task = DownloadLootboxData();
        }

        public bool IsDownloadingCompleted()
        {
            if (task == null)
            {
                log.Debug("task is null");
                return false;
            }
            
            return task.IsCompleted;
        }
        
        [CanBeNull]
        public LootboxModel GetLootboxModel()
        {
            // return new LootboxModel()
            // {
            //     Prizes = new List<ResourceModel>()
            //     {
            //         new ResourceModel()
            //         {
            //             ResourceTypeEnum = ResourceTypeEnum.HardCurrency,
            //             SerializedModel = ZeroFormatterSerializer.Serialize(new SoftCurrencyResourceModel
            //             {
            //                 Amount = 15
            //             })
            //         },
            //         // new ResourceModel()
            //         // {
            //         //     ResourceTypeEnum = ResourceTypeEnum.SoftCurrency,
            //         //     SerializedModel = ZeroFormatterSerializer.Serialize(new SoftCurrencyResourceModel
            //         //     {
            //         //         Amount = 15
            //         //     })
            //         // },
            //         // new ResourceModel()
            //         // {
            //         //     ResourceTypeEnum = ResourceTypeEnum.SoftCurrency,
            //         //     SerializedModel = ZeroFormatterSerializer.Serialize(new SoftCurrencyResourceModel
            //         //     {
            //         //         Amount = 15
            //         //     })
            //         // },
            //         // new ResourceModel()
            //         // {
            //         //     ResourceTypeEnum = ResourceTypeEnum.SoftCurrency,
            //         //     SerializedModel = ZeroFormatterSerializer.Serialize(new SoftCurrencyResourceModel
            //         //     {
            //         //         Amount = 15
            //         //     })
            //         // },
            //         // new ResourceModel()
            //         // {
            //         //     ResourceTypeEnum = ResourceTypeEnum.SoftCurrency,
            //         //     SerializedModel = ZeroFormatterSerializer.Serialize(new SoftCurrencyResourceModel
            //         //     {
            //         //         Amount = 15
            //         //     })
            //         // },
            //     }
            // };
            return task.Result;
        }
        
        private async Task<LootboxModel> DownloadLootboxData()
        {
            string url = NetworkGlobals.GetLootboxDataUrl;
            if (!PlayerIdStorage.TryGetServiceId(out string playerServiceId))
            {
                log.Error($"{nameof(DownloadLootboxData)} {nameof(playerServiceId)} is null");
                return null;
            }
            
            if (playerServiceId == null)
            {
                log.Error($"{nameof(playerServiceId)} is null");
                return null;
            }
            
            (string name, string value)[] fields =
            {
                (nameof(playerServiceId), playerServiceId)
            };

            try
            {
                byte[] data = await HttpWrapper.Post(url, fields);
                if (data != null && data.Length != 0)
                {
                    return ZeroFormatterSerializer.Deserialize<LootboxModel>(data);
                }

                log.Error("Пустой ответ от сервера");
            }
            catch (Exception e)
            {
                log.Error("Упало при скачивании данных о лутбоксе " + e.Message);
            }

            return null;
        }
    }
}