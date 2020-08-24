using Entitas;
using Entitas.CodeGeneration.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.ECS.Components
{
    [Game, Input]
    public sealed class IdComponent : IComponent
    {
        [PrimaryEntityIndex] public ushort value;
    }

    [Game]
    public class HealthComponent : IComponent
    {
        public int value;
    }

    [Game]
    public class MaxHealthComponent : IComponent
    {
        public int value;
    }

    /// <summary>
    /// Находится на сущности с view полоски жизни
    /// </summary>
    [Game]
    public class HealthBarComponent : IComponent
    {
        public Slider slider;
        public TextMeshProUGUI username;
        public TextMeshProUGUI healthPoints;
        public GameEntity parent;
    }

    [Game]
    public class NeedHealthBar : IComponent
    {
    }

    [Game]
    public class HealthBarParent : IComponent
    {
        public GameEntity healthBarEntity;
    }

    [Game]
    public class LocalPrediction : IComponent
    {
        
    }

}