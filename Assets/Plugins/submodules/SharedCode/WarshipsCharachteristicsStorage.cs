using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plugins.submodules.SharedCode
{
    [Serializable]
    [CreateAssetMenu(menuName = "ScriptableObjects/WarshipScriptableObject", order = 1)]
    public class WarshipSO : ScriptableObject
    {
        public float maxVelocity;
        public float maxHealth;
        public GameObject prefab;
        public ViewTypeEnum viewTypeEnum;
        public float angularVelocity;

        public override string ToString()
        {
            return $"maxSpeed={maxVelocity}, maxHealth={maxHealth}," +
                   $" viewTypeEnum={viewTypeEnum}, angularVelocity={angularVelocity}";
        }
    }
}