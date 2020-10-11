using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Code.Scenes.LootboxScene.Scripts
{
    public class LootboxUiStorage : MonoBehaviour
    {
        [Header("Корень для всего, что спавнится")]
        public GameObject resourcesRoot;
        
        [Header("Меню \"ITEMS LEFT\"")]
        public GameObject itemsLeftRoot;
        public Text itemsLeftText;
        
        [Header("Префабы для анимаций начисления ресурсов")]
        public GameObject softCurrencyPrefab;
        public GameObject hardCurrencyPrefab;
        public GameObject warshipPowerPointsPrefab;
        public GameObject lootboxPrefab;
    }
}