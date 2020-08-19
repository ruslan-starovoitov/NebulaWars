using System;
using System.Collections;
using Code.Common;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LootboxScene.Scripts
{
    public class AnimationInitializer : MonoBehaviour
    {
        private LootboxEcsController lootboxEcsController;
        private LootboxSceneSwitcher lootboxSceneSwitcher;
        private readonly TimeSpan maxWaitingTime = TimeSpan.FromSeconds(3);
        private readonly ILog log = LogManager.CreateLogger(typeof(AnimationInitializer));

        protected void Awake()
        {
            lootboxEcsController = FindObjectOfType<LootboxEcsController>()
                        ?? throw new NullReferenceException(nameof(lootboxEcsController));
            lootboxSceneSwitcher = FindObjectOfType<LootboxSceneSwitcher>()
                        ?? throw new NullReferenceException(nameof(lootboxSceneSwitcher));
        }

        private void Start()
        {
            StartCoroutine(StartAnimation());
        }

        private IEnumerator StartAnimation()
        {
            if (ResourcesAccrualStorage.Instance.IsLootboxNeeded)
            {
                lootboxEcsController.ShowLootbox();
            }
            else
            {
                lootboxEcsController.DisableItemsLeftMenu();
            }
            
            DateTime delayTime = DateTime.UtcNow + maxWaitingTime;
            yield return new WaitUntil(()=>
            {
                return ResourcesAccrualStorage.Instance.ResourceModels != null || DateTime.UtcNow > delayTime;
            });

            if (ResourcesAccrualStorage.Instance.ResourceModels != null)
            {
                lootboxEcsController.SetResourceModels(ResourcesAccrualStorage.Instance.ResourceModels );
            }
            else
            {
                UiSoundsManager.Instance().PlayError();
                lootboxSceneSwitcher.LoadLobbyScene();
            }
        }
    }
}