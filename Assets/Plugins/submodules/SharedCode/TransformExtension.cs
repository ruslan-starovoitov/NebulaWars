using System.Collections.Generic;
using UnityEngine;

namespace SharedSimulationCode
{
    
    public static class TransformExtension
    {
        public static List<Transform> GetChildrenWithTag(this Transform parent, string tag)
        {
            List<Transform> result = new List<Transform>(2);
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.CompareTag(tag))
                {
                    result.Add(child.gameObject.transform);
                }
                if (child.childCount > 0)
                {
                    GetChildrenWithTag(child, tag);
                }
            }

            return result;
        }
    }
}