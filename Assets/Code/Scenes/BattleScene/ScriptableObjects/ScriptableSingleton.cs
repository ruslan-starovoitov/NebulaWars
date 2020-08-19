using System;
using System.Linq;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.BattleScene.ScriptableObjects
{
    public abstract class ScriptableSingleton<T> : ScriptableObject 
        where T : ScriptableSingleton<T>
    {
        protected static T instance;
        private static ILog log = LogManager.CreateLogger(typeof(ScriptableSingleton<T>));
        
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Type type = typeof(T);
                    T[] instances = Resources.LoadAll<T>(string.Empty);
                    if (instances.Length > 1)
                    {
                        log.Error($"[ScriptableSingleton] Multiple instances of {type.Name} found!");
                    }
                    else
                    {
                        instance = instances.FirstOrDefault();
                        if (instance == null)
                        {
                            throw new Exception($"Не удалось найти {type.Name}.");
                        }
                    }
                }

                return instance;
            }
        }
    }
}
