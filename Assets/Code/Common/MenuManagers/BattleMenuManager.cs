using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Code.Scenes.BattleScene.Experimental;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Common
{
    [RequireComponent(typeof(BaseMenuManager))]
    public class BattleMenuManager : Singleton<BattleMenuManager>
    {
        [Header("Объекты для настройки цвета")]
        [SerializeField] private Dropdown colorsDropdown;
        [Header("Объекты \"замков\" для движения")]
        [SerializeField] private GameObject movementLocked;
        [SerializeField] private GameObject movementUnlocked;
        [Header("Объекты \"замков\" для атаки")]
        [SerializeField] private GameObject attackLocked;
        [SerializeField] private GameObject attackUnlocked;
        private JoysticksManager joysticksManager;
        private UiSoundsManager uiSoundsManager;
        // private TeamsColorManager colorManager;

        protected override void Awake()
        {
            base.Awake();

            joysticksManager = JoysticksManager.Instance();
            uiSoundsManager = UiSoundsManager.Instance();
            // colorManager = TeamsColorManager.Instance();

            var colorsOptions = new List<Dropdown.OptionData>();
            // var names = Enum.GetNames(typeof(TeamsColorManager.ColorsMode));
            // foreach (var optionName in names)
            // {
            //     var prettifiedName = Regex.Replace(optionName, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
            //     colorsOptions.Add(new Dropdown.OptionData(prettifiedName));
            // }
            // colorsDropdown.options = colorsOptions;
            // colorsDropdown.SetValueWithoutNotify((int)colorManager.TeamsColorsMode);

            if (joysticksManager.MovementType != JoystickType.Fixed)
            {
                movementLocked.SetActive(false);
                movementUnlocked.SetActive(true);
            }
            else
            {
                movementLocked.SetActive(true);
                movementUnlocked.SetActive(false);
            }
            if (joysticksManager.AttackType != JoystickType.Fixed)
            {
                attackLocked.SetActive(false);
                attackUnlocked.SetActive(true);
            }
            else
            {
                attackLocked.SetActive(true);
                attackUnlocked.SetActive(false);
            }
        }

        public void Dropdown_Color_Index_Changed(int value)
        {
            // colorManager.TeamsColorsMode = (TeamsColorManager.ColorsMode) value;
            uiSoundsManager.PlayClick();
        }

        public void Button_Lock_Movement_Click()
        {
            if (joysticksManager.MovementType == JoystickType.Fixed)
            {
                joysticksManager.MovementType = JoystickType.Dynamic;
                movementLocked.SetActive(false);
                movementUnlocked.SetActive(true);
            }
            else
            {
                joysticksManager.MovementType = JoystickType.Fixed;
                movementLocked.SetActive(true);
                movementUnlocked.SetActive(false);
            }
            uiSoundsManager.PlayClick();
        }

        public void Button_Lock_Attack_Click()
        {
            if (joysticksManager.AttackType == JoystickType.Fixed)
            {
                joysticksManager.AttackType = JoystickType.Dynamic;
                attackLocked.SetActive(false);
                attackUnlocked.SetActive(true);
            }
            else
            {
                joysticksManager.AttackType = JoystickType.Fixed;
                attackLocked.SetActive(true);
                attackUnlocked.SetActive(false);
            }
            uiSoundsManager.PlayClick();
        }
    }
}