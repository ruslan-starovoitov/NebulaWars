using System.Threading.Tasks;
using Code.Common;
using Plugins.submodules.SharedCode;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Должен показывать затемнение экрана при смерти игрока.
    /// </summary>
    public class PlayerDeathUiController : MonoBehaviour
    {
        [SerializeField] private GameObject imageForPlayerDeath;

        private void Awake()
        {
            imageForPlayerDeath.SetActive(false);
        }

        public void ShowImageAnimation()
        {
            UnityThread.Execute(async () =>
            {
                imageForPlayerDeath.SetActive(true);
                Image image = imageForPlayerDeath.GetComponent<Image>();
                int maxAlpha = 45;
                for (int currentAlpha = maxAlpha; currentAlpha > 0; currentAlpha--)
                {
                    var currentColor = image.color;
                    image.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentAlpha);
                    await Task.Delay(15);
                }
            });
        }
    }
}