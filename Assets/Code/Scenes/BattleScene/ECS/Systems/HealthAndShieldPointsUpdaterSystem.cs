using Code.Scenes.BattleScene.Experimental.Approximation;
using Entitas;
using System;
using System.Collections.Generic;
using System.Threading;
using Code.Common;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.ECS.Systems
{
    public class HealthAndShieldPointsUpdaterSystem : IExecuteSystem, ITearDownSystem
    {
        private static volatile float healthPoints;
        private static volatile float maxHealthPoints;
        private static volatile float shieldPoints;
        private static volatile float maxShieldPoints;
        private static volatile bool wasChanged;
        private bool _shieldEnabled;
        private readonly Slider _healthSlider;
        private readonly Text _healthText;
        private readonly Slider _shieldSlider;
        private readonly Text _shieldText;
        private readonly IApproximator<float> approximator;
        private const float vignetteStartValue = 1000f;
        private readonly Vignette vignette;
        private const float vignetteWeight = 0.4f;
        private static readonly Vector3 deltaPosition = new Vector3(0f, -10f, 0f);
        private static readonly Vector3 bigScale = new Vector3(1f, 1.25f, 1f);
        private readonly ILog log = LogManager.CreateLogger(typeof(HealthAndShieldPointsUpdaterSystem));

        public HealthAndShieldPointsUpdaterSystem(Slider healthSlider, Text healthText, Slider shieldSlider, Text shieldText, IApproximator<float> pointsApproximator, Vignette vignette)
        {
            if(healthSlider == null)
                throw new Exception($"{nameof(HealthAndShieldPointsUpdaterSystem)} {nameof(healthSlider)} was null");
            if (healthText == null)
                throw new Exception($"{nameof(HealthAndShieldPointsUpdaterSystem)} {nameof(healthText)} was null");
            if (shieldSlider == null)
                throw new Exception($"{nameof(HealthAndShieldPointsUpdaterSystem)} {nameof(shieldSlider)} was null");
            if (shieldText == null)
                throw new Exception($"{nameof(HealthAndShieldPointsUpdaterSystem)} {nameof(shieldText)} was null");

            this.vignette = vignette;

            approximator = pointsApproximator;

            _healthSlider = healthSlider;
            _healthText = healthText;
            _shieldSlider = shieldSlider;
            _shieldText = shieldText;
            maxHealthPoints = vignetteStartValue;
            maxShieldPoints = 0f;
            _shieldSlider.gameObject.SetActive(false);
            _healthSlider.transform.localPosition += deltaPosition;
            _healthSlider.transform.localScale = bigScale;
            vignette.intensity.value = 0f;
            vignette.enabled.value = true;

            approximator.Set(new Dictionary<ushort, float>(2) { { 0, 0f }, { 1, 0f } }, Time.time - Time.deltaTime);
            approximator.Set(new Dictionary<ushort, float>(2) { { 0, 0f }, { 1, 0f } }, Time.time);
        }
            
        public static void SetHealthPoints(float healthPointsArg)
        {
            healthPoints = healthPointsArg;
            if (healthPoints < 0f) healthPoints = 0f;
            wasChanged = true;
        }
        
        public static void SetMaxHealthPoints(float maxHealthPointsArg)
        {
            maxHealthPoints = maxHealthPointsArg;
            wasChanged = true;
        }

        public static void SetShieldPoints(float shieldPointsArg)
        {
            shieldPoints = shieldPointsArg;
            if (shieldPoints < 0f) shieldPoints = 0f;
            wasChanged = true;
        }

        public static void SetMaxShieldPoints(float maxShieldPointsArg)
        {
            maxShieldPoints = maxShieldPointsArg;
            wasChanged = true;
        }

        public void Execute()
        {
            var dict = approximator.Get(Time.time);
            var healthPoints = dict[0];
            var shieldPoints = dict[1];

            if (wasChanged)
            {
                wasChanged = false;
                _healthSlider.maxValue = maxHealthPoints;

                if (maxShieldPoints > 0f)
                {
                    if (!_shieldEnabled)
                    {
                        _shieldEnabled = true;
                        _shieldSlider.gameObject.SetActive(true);
                        _healthSlider.transform.localPosition -= deltaPosition;
                        _healthSlider.transform.localScale = new Vector3(1f, 1f, 1f);
                    }

                    _shieldSlider.maxValue = maxShieldPoints;

                }
                else if (_shieldEnabled)
                {
                    _shieldSlider.gameObject.SetActive(false);
                    _healthSlider.transform.localPosition += deltaPosition;
                    _healthSlider.transform.localScale = bigScale;
                    _shieldEnabled = false;
                    Interlocked.Exchange(ref HealthAndShieldPointsUpdaterSystem.shieldPoints, 0f);
                }

                approximator.Set(new Dictionary<ushort, float>(2)
                {
                    { 0, HealthAndShieldPointsUpdaterSystem.healthPoints },
                    { 1, HealthAndShieldPointsUpdaterSystem.shieldPoints }
                }, Time.time);
            }

            _healthSlider.value = healthPoints;
            _healthText.text = healthPoints.ToString("F0") + '/' + maxHealthPoints.ToString("F0");

            var vignetteHealthRatio = healthPoints / vignetteStartValue;
            vignetteHealthRatio *= vignetteHealthRatio;

            vignette.intensity.value = (1f - vignetteHealthRatio) * vignetteWeight;

            if (_shieldEnabled)
            {
                _shieldSlider.value = shieldPoints;
                _shieldText.text = shieldPoints.ToString("F0") + '/' + maxShieldPoints.ToString("F0");
            }

            // Исправляет баг Unity - иначе слайдер может исчезать.
            var healthLocalPos = _healthSlider.fillRect.transform.localPosition;
            if (float.IsNaN(healthLocalPos.x))
            {
                log.Warn($"HealthSlider x was NaN.");

                _healthSlider.fillRect.transform.localPosition = Vector3.zero;
                _healthSlider.value = 0f;
                _healthSlider.value = healthPoints;
            }

            // Исправляет баг Unity - иначе слайдер может съезжать.
            var left = _healthSlider.fillRect.offsetMin.x;
            if (left > 0f)
            {
                log.Warn($"HealthSlider left had bad value: {left}.");
                _healthSlider.fillRect.offsetMin = Vector2.zero;
                _healthSlider.fillRect.offsetMax = Vector2.zero;
            }
        }

        public void TearDown()
        {
            vignette.intensity.value = 0f;
            vignette.enabled.value = false;
            approximator.Clear();
        }
    }
}