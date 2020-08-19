using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Common
{
    [RequireComponent(typeof(Button))]
    public class MenuButtonListener : MonoBehaviour
    {
        [SerializeField] private BaseMenuManager menu;
        private readonly ILog log = LogManager.CreateLogger(typeof(MenuButtonListener));
        
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(menu.ShowMenu);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                log.Info("Нажата клавиша Escape");
                menu.SwitchMenu();
            }
        }
    }
}