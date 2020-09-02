using System;
using System.Collections;
using Code.Common;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LootboxScene.PrefabScripts
{
    public class LootboxOpeningController : MonoBehaviour , ILootboxOpeningController
    {
        private bool isOpened;
        private Transform closedBox;
        private Transform openedBox;
        [SerializeField] private GameObject effectPrefab;
        private readonly ILog log = LogManager.CreateLogger(typeof(LootboxOpeningController));

        private void Awake()
        {
            closedBox = transform.Find("Sprite_ClosedLootbox");
            openedBox = transform.Find("Sprite_OpenedLootbox");

            if (effectPrefab == null)
            {
                throw new NullReferenceException(nameof(effectPrefab));
            }
        }

        private void Start()
        {
            isOpened = false;
            closedBox.gameObject.SetActive(true);
            openedBox.gameObject.SetActive(false);
        }

        public void InitClose()
        {
            closedBox.gameObject.SetActive(true);
            openedBox.gameObject.SetActive(false);
        }
        
        public void StartLootboxOpening(Action callback, Transform parent)
        {
            if (isOpened) 
            {
                log.Error("Лутбокс уже открыт.");
                return;
            }
        
            StartCoroutine(OpenAnimation(callback, parent));
        }
    
        private IEnumerator OpenAnimation(Action callback, Transform parent)
        {
            isOpened = true;
            UiSoundsManager.Instance().PlayLootbox();
            yield return new WaitForSeconds(0.2f);

            closedBox.gameObject.SetActive(false);
            Transform transform1 = closedBox.transform;
            Vector3 position = transform1.position;
            Quaternion rotation = transform1.rotation;
            openedBox.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            Instantiate(effectPrefab, position, rotation, parent);
            float shakeDuration = 0.1f;
            CameraShake.myCameraShake.ShakeCamera(0.3f, shakeDuration);
            yield return new WaitForSeconds(shakeDuration);
            //Время задержки перед заменой лутбокса на ресурс
            yield return new WaitForSeconds(1);
            callback?.Invoke();
        }
    }
}