using System;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode
{
    public class WarshipsCharacteristicsStorage : MonoBehaviour
    {
        [SerializeField] private GameObject starSparrowPrefab;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipsCharacteristicsStorage));
        
        private void Awake()
        {
            //todo SerializeField не сохраняется в Linux сборке
            if (starSparrowPrefab == null)
            {
                throw new Exception("Префаб пуст");
            }
            else
            {
                log.Debug("Префаб найден");
            }
        }
    
        public WarshipSO Get(ViewTypeEnum viewTypeEnum)
        {
            if (viewTypeEnum == ViewTypeEnum.StarSparrow1)
            {
                var warshipScriptableObject = ScriptableObject.CreateInstance<WarshipSO>();
                warshipScriptableObject.prefab = starSparrowPrefab;
                warshipScriptableObject.angularVelocity = 10;
                warshipScriptableObject.maxHealth = 1500;
                warshipScriptableObject.maxVelocity = 10;
                warshipScriptableObject.viewTypeEnum = ViewTypeEnum.StarSparrow1;
                return warshipScriptableObject;
            }

            throw new Exception($"Запрос объекта, которого нет. {viewTypeEnum}");
        }
    }
}