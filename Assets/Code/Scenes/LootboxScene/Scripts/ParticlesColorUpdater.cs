using System;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LootboxScene.Scripts
{
    public class ParticlesColorUpdater : MonoBehaviour
    {
        [SerializeField] private ParticleSystem dots;
        [SerializeField] private ParticleSystem circlesOnAWater;
        private readonly ILog log = LogManager.CreateLogger(typeof(ParticlesColorUpdater));
        
        private void Awake()
        {
            if (circlesOnAWater == null)
            {
                throw new NullReferenceException(nameof(circlesOnAWater));
            }
            if (dots == null)
            {
                throw new NullReferenceException(nameof(dots));
            }
        }

        public void SetStartColor(Color color)
        {
            {
                ParticleSystem.MainModule mainModule = circlesOnAWater.main;
                mainModule.startColor = color;
            }

            {
                ParticleSystem.MainModule mainModule = dots.main;
                mainModule.startColor = color;
            }
        }
    }
}