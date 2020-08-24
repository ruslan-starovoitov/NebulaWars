// using Entitas;
//
// // ReSharper disable once CheckNamespace
// namespace Code.BattleScene.ECS.Systems
// {
//     public class AbilityInputClearingSystem : ICleanupSystem
//     {
//         private readonly InputContext inputContext;
//
//         public AbilityInputClearingSystem(Contexts contexts)
//         {
//             inputContext = contexts.input;
//         }
//
//         public void Cleanup()
//         {
//             inputContext.isTryingToUseAbility = false;
//         }
//     }
// }