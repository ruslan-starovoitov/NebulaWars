using System;
using Code.Common;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LootboxScene.PrefabScripts.Wpp
{
    [RequireComponent(typeof(AudioSource))]
    public class LightningSound : MonoBehaviour
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(LightningSound));
        
        private void Awake()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                throw new NullReferenceException(nameof(audioSource));
            }

            if (audioSource.clip==null)
            {
                throw new NullReferenceException(nameof(audioSource.clip));
            }

            audioSource.volume = SoundManager.Instance().InterfaceVolume/5;
        }
    }
}