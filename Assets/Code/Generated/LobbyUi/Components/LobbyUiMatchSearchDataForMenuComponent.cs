//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class LobbyUiContext {

    public LobbyUiEntity matchSearchDataForMenuEntity { get { return GetGroup(LobbyUiMatcher.MatchSearchDataForMenu).GetSingleEntity(); } }
    public Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent matchSearchDataForMenu { get { return matchSearchDataForMenuEntity.matchSearchDataForMenu; } }
    public bool hasMatchSearchDataForMenu { get { return matchSearchDataForMenuEntity != null; } }

    public LobbyUiEntity SetMatchSearchDataForMenu(int newNumberOfPlayersInMatch, int newNumberOfPlayersInQueue) {
        if (hasMatchSearchDataForMenu) {
            throw new Entitas.EntitasException("Could not set MatchSearchDataForMenu!\n" + this + " already has an entity with Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent!",
                "You should check if the context already has a matchSearchDataForMenuEntity before setting it or use context.ReplaceMatchSearchDataForMenu().");
        }
        var entity = CreateEntity();
        entity.AddMatchSearchDataForMenu(newNumberOfPlayersInMatch, newNumberOfPlayersInQueue);
        return entity;
    }

    public void ReplaceMatchSearchDataForMenu(int newNumberOfPlayersInMatch, int newNumberOfPlayersInQueue) {
        var entity = matchSearchDataForMenuEntity;
        if (entity == null) {
            entity = SetMatchSearchDataForMenu(newNumberOfPlayersInMatch, newNumberOfPlayersInQueue);
        } else {
            entity.ReplaceMatchSearchDataForMenu(newNumberOfPlayersInMatch, newNumberOfPlayersInQueue);
        }
    }

    public void RemoveMatchSearchDataForMenu() {
        matchSearchDataForMenuEntity.Destroy();
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class LobbyUiEntity {

    public Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent matchSearchDataForMenu { get { return (Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent)GetComponent(LobbyUiComponentsLookup.MatchSearchDataForMenu); } }
    public bool hasMatchSearchDataForMenu { get { return HasComponent(LobbyUiComponentsLookup.MatchSearchDataForMenu); } }

    public void AddMatchSearchDataForMenu(int newNumberOfPlayersInMatch, int newNumberOfPlayersInQueue) {
        var index = LobbyUiComponentsLookup.MatchSearchDataForMenu;
        var component = (Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent)CreateComponent(index, typeof(Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent));
        component.numberOfPlayersInMatch = newNumberOfPlayersInMatch;
        component.numberOfPlayersInQueue = newNumberOfPlayersInQueue;
        AddComponent(index, component);
    }

    public void ReplaceMatchSearchDataForMenu(int newNumberOfPlayersInMatch, int newNumberOfPlayersInQueue) {
        var index = LobbyUiComponentsLookup.MatchSearchDataForMenu;
        var component = (Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent)CreateComponent(index, typeof(Code.Scenes.LobbyScene.ECS.MatchSearchDataForMenuComponent));
        component.numberOfPlayersInMatch = newNumberOfPlayersInMatch;
        component.numberOfPlayersInQueue = newNumberOfPlayersInQueue;
        ReplaceComponent(index, component);
    }

    public void RemoveMatchSearchDataForMenu() {
        RemoveComponent(LobbyUiComponentsLookup.MatchSearchDataForMenu);
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
public sealed partial class LobbyUiMatcher {

    static Entitas.IMatcher<LobbyUiEntity> _matcherMatchSearchDataForMenu;

    public static Entitas.IMatcher<LobbyUiEntity> MatchSearchDataForMenu {
        get {
            if (_matcherMatchSearchDataForMenu == null) {
                var matcher = (Entitas.Matcher<LobbyUiEntity>)Entitas.Matcher<LobbyUiEntity>.AllOf(LobbyUiComponentsLookup.MatchSearchDataForMenu);
                matcher.componentNames = LobbyUiComponentsLookup.componentNames;
                _matcherMatchSearchDataForMenu = matcher;
            }

            return _matcherMatchSearchDataForMenu;
        }
    }
}
