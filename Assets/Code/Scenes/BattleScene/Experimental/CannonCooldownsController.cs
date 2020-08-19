using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.Cooldown;
using UnityEngine;

namespace Code.Scenes.BattleScene.Experimental
{
    public class CannonCooldownsController : MonoBehaviour
    {
        [SerializeField]
        private CooldownInfo prototype;
        private CooldownInfo[] infos;
        private const float infoWidth = 84f;
        private const float halfInfoWidth = infoWidth * 0.5f;

        public void Load(WeaponInfo[] weaponsInfo)
        {
            if (infos == null)
            {
                CreateInfos(weaponsInfo.Length);
            }
            else if(infos.Length != weaponsInfo.Length)
            {
                RecreateInfos(weaponsInfo.Length);
            }

            for (var i = 0; i < weaponsInfo.Length; i++)
            {
                infos[i].Load(weaponsInfo[i]);
            }
        }

        public void SetCooldowns(float[] cooldowns)
        {
            if(infos == null) return;
            for (int i = 0; i < cooldowns.Length && i < infos.Length; i++)
            {
                infos[i].SetCooldown(cooldowns[i]);
            }
        }

        private void CreateInfos(int count)
        {
            infos = new CooldownInfo[count];

            var offset = -halfInfoWidth * (count - 1);

            for (var i = 0; i < count; i++)
            {
                var info = Instantiate(prototype, transform);
                var currentOffset = offset + i * infoWidth;
                info.transform.localPosition += new Vector3(currentOffset, 0f, 0f);
                infos[i] = info;
            }
        }

        private void RecreateInfos(int count)
        {
            foreach (var info in infos)
            {
                Destroy(info.gameObject);
            }

            CreateInfos(count);
        }
    }
}
