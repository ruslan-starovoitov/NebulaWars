using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.ECS.Warships.Utils
{
    public static class CurrentWarshipTypeStorage
    {
        private const string Key = "warshipIndex";
        private static readonly ILog Log = LogManager.CreateLogger(typeof(CurrentWarshipTypeStorage));
        
        public static void WriteWarshipType(WarshipTypeEnum warshipTypeEnum)
        {
            // Log.Debug($"Установка типа корабля {warshipTypeEnum.ToString()}");
            PlayerPrefs.SetInt(Key, (int) warshipTypeEnum);
            PlayerPrefs.Save();
        }

        public static WarshipTypeEnum ReadWarshipType()
        {
            var result = (WarshipTypeEnum) PlayerPrefs.GetInt(Key, (int) WarshipTypeEnum.Hare);
            // Log.Debug($"Чтение типа корабля  {result.ToString()}");
            return result ;
        }
    }
}