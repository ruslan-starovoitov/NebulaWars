using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Components.Game
{
    [Game]
    public class Transform2DComponent : IComponent
    {
        public Vector3 position;
        public float angle;
    }
}
