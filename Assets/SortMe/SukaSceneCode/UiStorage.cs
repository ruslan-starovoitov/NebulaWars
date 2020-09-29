using Code.Scenes.LobbyScene.Scripts;
using Code.Scenes.LobbyScene.Scripts.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.SukaScene
{
    public class UiStorage : MonoBehaviour
    {
        [SerializeField] private Button currencyButton;
        [SerializeField] private Button confirmAllButton;
        private IPurchasingService purchaser;

        private void Awake()
        {
            purchaser = FindObjectOfType<PurchasingServiceStub>();
        }

        private void Start()
        {
            currencyButton.onClick.RemoveAllListeners();
            currencyButton.onClick.AddListener(() =>
            {
                purchaser.BuyCurrency();
            }); 
            confirmAllButton.onClick.RemoveAllListeners();
            confirmAllButton.onClick.AddListener( () =>
            {
                purchaser.ConfirmAll();
            });
        }
    }
}
