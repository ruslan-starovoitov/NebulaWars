using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Cooldown;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.Experimental
{
    public class CooldownInfo : MonoBehaviour
    {
        [SerializeField]
        private Slider cooldownSlider;
        [SerializeField]
        private Image bulletPreview;

        private float totalCooldown;
        private static readonly Color reloadedColor = Color.white;
        private static readonly Color reloadingColor = new Color(1f, 1f, 1f, 0.75f);

        public void Load(WeaponInfo info)
        {
            bulletPreview.sprite = PreviewsManager.GetSprite(info.ViewType);
            Load(info.Cooldown);
        }

        public void Load(float maxCooldown)
        {
            totalCooldown = maxCooldown;
            cooldownSlider.maxValue = totalCooldown;
            cooldownSlider.value = totalCooldown;
        }

        public void SetCooldown(float cooldown)
        {
            if (cooldown > 0f)
            {
                cooldownSlider.value = totalCooldown - cooldown;
                bulletPreview.color = reloadingColor;
            }
            else
            {
                cooldownSlider.value = totalCooldown;
                bulletPreview.color = reloadedColor;
            }
        }
    }
}
