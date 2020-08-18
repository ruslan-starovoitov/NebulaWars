//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Code.Scenes.BattleScene.ECS.Components.HealthBarParent healthBarParent { get { return (Code.Scenes.BattleScene.ECS.Components.HealthBarParent)GetComponent(GameComponentsLookup.HealthBarParent); } }
    public bool hasHealthBarParent { get { return HasComponent(GameComponentsLookup.HealthBarParent); } }

    public void AddHealthBarParent(GameEntity newHealthBarEntity) {
        var index = GameComponentsLookup.HealthBarParent;
        var component = (Code.Scenes.BattleScene.ECS.Components.HealthBarParent)CreateComponent(index, typeof(Code.Scenes.BattleScene.ECS.Components.HealthBarParent));
        component.healthBarEntity = newHealthBarEntity;
        AddComponent(index, component);
    }

    public void ReplaceHealthBarParent(GameEntity newHealthBarEntity) {
        var index = GameComponentsLookup.HealthBarParent;
        var component = (Code.Scenes.BattleScene.ECS.Components.HealthBarParent)CreateComponent(index, typeof(Code.Scenes.BattleScene.ECS.Components.HealthBarParent));
        component.healthBarEntity = newHealthBarEntity;
        ReplaceComponent(index, component);
    }

    public void RemoveHealthBarParent() {
        RemoveComponent(GameComponentsLookup.HealthBarParent);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherHealthBarParent;

    public static Entitas.IMatcher<GameEntity> HealthBarParent {
        get {
            if (_matcherHealthBarParent == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.HealthBarParent);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherHealthBarParent = matcher;
            }

            return _matcherHealthBarParent;
        }
    }
}