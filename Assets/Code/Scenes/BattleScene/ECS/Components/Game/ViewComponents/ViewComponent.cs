using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Components.Game.ViewComponents
{
    [Game, WppAccrual]
    public sealed class ViewComponent : IComponent
    {
        public GameObject gameObject;
    }

    [Game]
    public class ViewTransformComponent : IComponent
    {
        public Transform value;
    }
}
