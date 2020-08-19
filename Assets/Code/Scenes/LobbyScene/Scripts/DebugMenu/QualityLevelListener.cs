using System.Collections;
using System.Collections.Generic;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.LobbyScene.Scripts.DebugMenu
{
    public class QualityLevelListener : MonoBehaviour
    {
        private static bool loaded = false;
        private static Resolution maxResolution;
        private static int maxDivider;
        private static int ResolutionMultiplier;
        private int newMultiplier;
        private const string ButtonText = "Apply resolution";
        private const int DelayBeforeCancel = 10;
        private readonly ILog log = LogManager.CreateLogger(typeof(QualityLevelListener));
        private Dropdown qualityDropdown;
        private Slider resolutionSlider;
        private Dropdown resolutionDropdown;
        private Button resolutionApplyButton;
        private Text resolutionApplyText;
        private IEnumerator currentCoroutine;

        private void Awake()
        {
            var uiStorage = FindObjectOfType<DebugMenuUiStorage>();
            qualityDropdown = uiStorage.qualityDropdown;
            resolutionSlider = uiStorage.resolutionSlider;
            resolutionDropdown = uiStorage.resolutionDropdown;
            resolutionApplyButton = uiStorage.resolutionApplyButton;
            resolutionApplyText = resolutionApplyButton.GetComponentInChildren<Text>();
            resolutionApplyButton.interactable = false;

            List<Dropdown.OptionData> qualityOptions = new List<Dropdown.OptionData>();
            foreach (var qualityLevelName in QualitySettings.names)
            {
                qualityOptions.Add(new Dropdown.OptionData(qualityLevelName));
            }

            qualityDropdown.options = qualityOptions;

            qualityDropdown.onValueChanged.AddListener(SetQualityLevel);

            var qualityLevel = QualitySettings.GetQualityLevel();

            qualityDropdown.value = qualityLevel;
            log.Info("текущее значение "+QualitySettings.names[qualityLevel]);

            if (!loaded)
            {
                loaded = true;
                maxResolution = Screen.currentResolution;
                var width = maxResolution.width;
                var height = maxResolution.height;
                maxDivider = Gcd(width, height);
                // Нестандартный размер экрана / экран "обрезан":
                if (maxDivider == 1) maxDivider = QualitySettings.names.Length * 20;

                ResolutionMultiplier = PlayerPrefs.GetInt(nameof(ResolutionMultiplier),
                    maxDivider
                    // * (qualityLevel + 1) / QualitySettings.names.Length
                    );
                SetResolution(ResolutionMultiplier, false);
            }

            resolutionSlider.maxValue = maxDivider;
            resolutionSlider.SetValueWithoutNotify(ResolutionMultiplier);

            var atomicWidth = maxResolution.width / maxDivider;
            var atomicHeight = maxResolution.height / maxDivider;
            List<Dropdown.OptionData> resolutionOptions = new List<Dropdown.OptionData>(maxDivider);
            for (var i = maxDivider; i > 0; i--)
            {
                var width = atomicWidth * i;
                var height = atomicHeight * i;
                resolutionOptions.Add(new Dropdown.OptionData($"{width}x{height}"));
            }

            resolutionDropdown.options = resolutionOptions;
            resolutionDropdown.SetValueWithoutNotify(maxDivider - ResolutionMultiplier);
        }

        private void OnDestroy()
        {
            PlayerPrefs.SetInt(nameof(ResolutionMultiplier), ResolutionMultiplier);
            PlayerPrefs.Save();
        }

        private void SetResolution(int value, bool startCoroutine = true)
        {
            if(currentCoroutine != null) StopCoroutine(currentCoroutine);
            newMultiplier = value;
            var width = maxResolution.width;
            var height = maxResolution.height;
            Screen.SetResolution(width * newMultiplier / maxDivider, height * newMultiplier / maxDivider, true);

            resolutionSlider.SetValueWithoutNotify(newMultiplier);
            resolutionDropdown.SetValueWithoutNotify(maxDivider - newMultiplier);

            if (!startCoroutine) return;
            currentCoroutine = WaitForApply();
            StartCoroutine(currentCoroutine);
            resolutionApplyButton.interactable = true;
        }

        private IEnumerator WaitForApply()
        {
            for (var i = DelayBeforeCancel; i > 0; i--)
            {
                resolutionApplyText.text = $"{ButtonText} ({i})";
                yield return new WaitForSeconds(1f);
            }
            SetResolution(ResolutionMultiplier, false);
            ApplyResolution();
        }

        public void OnResolutionChanged(float value)
        {
            SetResolution((int)value);
        }

        public void OnResolutionIndexChanged(int value)
        {
            SetResolution(maxDivider - value);
        }

        public void ApplyResolution()
        {
            StopCoroutine(currentCoroutine);
            ResolutionMultiplier = newMultiplier;
            resolutionApplyText.text = ButtonText;
            resolutionApplyButton.interactable = false;
        }

        private void SetQualityLevel(int levelIndex)
        {
            string levelName = QualitySettings.names[levelIndex];
            log.Info("новый уровень "+levelName);
            QualitySettings.SetQualityLevel(levelIndex, true);
        }

        /// <summary>
        /// Наибольший общий делитель двух чисел.
        /// </summary>
        /// <param name="a">Первое число.</param>
        /// <param name="b">Второе число.</param>
        /// <returns>НОД двух чисел.</returns>
        private static int Gcd(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }
    }
}