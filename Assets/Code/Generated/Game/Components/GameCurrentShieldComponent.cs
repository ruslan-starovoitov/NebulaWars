// //------------------------------------------------------------------------------
// // <auto-generated>
// //     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
// //
// //     Changes to this file may cause incorrect behavior and will be lost if
// //     the code is regenerated.
// // </auto-generated>
// //------------------------------------------------------------------------------
// public partial class GameContext {
//
//     public GameEntity currentShieldEntity { get { return GetGroup(GameMatcher.CurrentShield).GetSingleEntity(); } }
//
//     public bool isCurrentShield {
//         get { return currentShieldEntity != null; }
//         set {
//             var entity = currentShieldEntity;
//             if (value != (entity != null)) {
//                 if (value) {
//                     CreateEntity().isCurrentShield = true;
//                 } else {
//                     entity.Destroy();
//                 }
//             }
//         }
//     }
// }
//
// //------------------------------------------------------------------------------
// // <auto-generated>
// //     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
// //
// //     Changes to this file may cause incorrect behavior and will be lost if
// //     the code is regenerated.
// // </auto-generated>
// //------------------------------------------------------------------------------
// public partial class GameEntity {
//
//     static readonly Code.Scenes.BattleScene.ECS.Components.Game.CurrentShieldComponent currentShieldComponent = new Code.Scenes.BattleScene.ECS.Components.Game.CurrentShieldComponent();
//
//     public bool isCurrentShield {
//         get { return HasComponent(GameComponentsLookup.CurrentShield); }
//         set {
//             if (value != isCurrentShield) {
//                 var index = GameComponentsLookup.CurrentShield;
//                 if (value) {
//                     var componentPool = GetComponentPool(index);
//                     var component = componentPool.Count > 0
//                             ? componentPool.Pop()
//                             : currentShieldComponent;
//
//                     AddComponent(index, component);
//                 } else {
//                     RemoveComponent(index);
//                 }
//             }
//         }
//     }
// }
//
// //------------------------------------------------------------------------------
// // <auto-generated>
// //     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
// //
// //     Changes to this file may cause incorrect behavior and will be lost if
// //     the code is regenerated.
// // </auto-generated>
// //------------------------------------------------------------------------------
// public sealed partial class GameMatcher {
//
//     static Entitas.IMatcher<GameEntity> _matcherCurrentShield;
//
//     public static Entitas.IMatcher<GameEntity> CurrentShield {
//         get {
//             if (_matcherCurrentShield == null) {
//                 var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.CurrentShield);
//                 matcher.componentNames = GameComponentsLookup.componentNames;
//                 _matcherCurrentShield = matcher;
//             }
//
//             return _matcherCurrentShield;
//         }
//     }
// }
