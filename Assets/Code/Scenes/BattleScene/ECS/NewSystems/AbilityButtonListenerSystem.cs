using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.Scripts
{
    public class AbilityButtonListenerSystem:IExecuteSystem,ICleanupSystem
    {
        private readonly Contexts contexts;
        private bool abilityButtonIsPressed;

        public AbilityButtonListenerSystem(Contexts contexts)
        {
            this.contexts = contexts;
        }

        public void AbilityButton_OnClick()
        {
            abilityButtonIsPressed = true;
        }

        public void Execute()
        {
            //Возможно, стоит это вынести отдельно
            contexts.input.isTryingToUseAbility = abilityButtonIsPressed;
#if UNITY_EDITOR_WIN
            contexts.input.isTryingToUseAbility |= Input.GetKey(KeyCode.Space);
#endif

        }

        public void Cleanup()
        {
            abilityButtonIsPressed = false;
        }
    }
}