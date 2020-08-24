using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Scenes.BattleScene.ECS.Components.Input
{
    [ServerInput, Unique]
    public class AttackComponent:IComponent
    {
        public float angle;
    }
}
