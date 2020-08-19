using System;
using System.Collections.Generic;
using System.Linq;
using Code.Common;
using Code.Scenes.LootboxScene.PrefabScripts;
using Code.Scenes.LootboxScene.PrefabScripts.Wpp;
using Code.Scenes.LootboxScene.Scripts;
using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using ZeroFormatter;

namespace Code.Scenes.LootboxScene.ECS.Systems
{
    /// <summary>
    /// Показывает новый ресурс.
    /// </summary>
    public class ShowPrizeSystem:ReactiveSystem<LootboxEntity>
    {
        private readonly LootboxUiStorage uiStorage;
        private readonly ParticlesColorUpdater particlesColorUpdater;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShowPrizeSystem));

        public ShowPrizeSystem(Contexts contexts, ParticlesColorUpdater particlesColorUpdater, LootboxUiStorage uiStorage) 
            : base(contexts.lootbox)
        {
            this.uiStorage = uiStorage;
            this.particlesColorUpdater = particlesColorUpdater;
        }
        
        protected override ICollector<LootboxEntity> GetTrigger(IContext<LootboxEntity> context)
        {
            return context.CreateCollector(LootboxMatcher.ShowPrize.Added());
        }

        protected override bool Filter(LootboxEntity entity)
        {
            return entity.hasShowPrize;
        }

        protected override void Execute(List<LootboxEntity> entities)
        {
            //очистить
            ClearResources();
            //начать показывать текущий приз
            LootboxEntity currentPrize = entities.Last();
            ShowPrize(currentPrize);
        }

        private void ClearResources()
        {
            try
            {
                uiStorage.resourcesRoot.transform.DestroyAllChildren();
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace+" "+e.Message);
            }
        }

        private void ShowPrize(LootboxEntity currentPrize)
        {
            ShowPrizeComponent prize = currentPrize.showPrize;
            Transform parent = uiStorage.resourcesRoot.transform;
            
            switch (prize.resourceModel.ResourceTypeEnum)
            {
                case ResourceTypeEnum.SoftCurrency:
                {
                    var go = UnityEngine.Object.Instantiate(uiStorage.softCurrencyPrefab, parent);
                    var script = go.GetComponent<SoftCurrencyAccrual>();
                    SoftCurrencyResourceModel softCurrencyResourceModel =
                        ZeroFormatterSerializer.Deserialize<SoftCurrencyResourceModel>(prize.resourceModel
                            .SerializedModel);
                    script.StartAnimation(softCurrencyResourceModel.Amount);
                    particlesColorUpdater.SetStartColor(Color.blue);
                    break;
                }
                case ResourceTypeEnum.HardCurrency:
                {
                    var go = UnityEngine.Object.Instantiate(uiStorage.hardCurrencyPrefab, parent);
                    var script = go.GetComponent<HardCurrencyAccrual>();
                    HardCurrencyResourceModel hardCurrencyResourceModel =
                        ZeroFormatterSerializer.Deserialize<HardCurrencyResourceModel>(prize.resourceModel
                            .SerializedModel);
                    script.SetData(hardCurrencyResourceModel.Amount);

                    Color purple = new Color(209, 0, 4);
                    particlesColorUpdater.SetStartColor(purple);
                    break;
                }
                case ResourceTypeEnum.WarshipPowerPoints:
                {
                    var go = UnityEngine.Object.Instantiate(uiStorage.warshipPowerPointsPrefab, parent);
                    var script = go.GetComponent<WarshipPowerPointsAccrual>();
                    var wppModel =
                        ZeroFormatterSerializer.Deserialize<WarshipPowerPointsResourceModel>(prize.resourceModel
                            .SerializedModel);
                    
                   
                    if (wppModel.WarshipTypeEnum == 0)
                    {
                        throw new Exception("Не указан тип корабля");
                    }
                    
                    script.SetData(wppModel);
                    Color red = new Color(209, 0, 0);
                    particlesColorUpdater.SetStartColor(red);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}