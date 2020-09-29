using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.MapInitialization
{
    /// <summary>
    /// Создаёт сущность команду создать корабль
    /// </summary>
    public class WarshipSpawnerHelper
    {
        private readonly WarshipEntityFactory warshipEntityFactory;
        private readonly WarshipsCharacteristicsStorage warshipsCharacteristicsStorage;
        private readonly ILog log = LogManager.CreateLogger(typeof(WarshipSpawnerHelper));

        public WarshipSpawnerHelper(WarshipEntityFactory warshipEntityFactory,
            WarshipsCharacteristicsStorage warshipsCharacteristicsStorage)
        {
            this.warshipEntityFactory = warshipEntityFactory;
            this.warshipsCharacteristicsStorage = warshipsCharacteristicsStorage;
        }
        
        public ServerGameEntity CreateWarship(Vector3 position, ushort tmpPlayerId, int accountId, 
            ViewTypeEnum viewTypeEnum)
        {
            WarshipSO warshipSo = warshipsCharacteristicsStorage.Get(viewTypeEnum);
            var entity = warshipEntityFactory.Create(position, tmpPlayerId, accountId, viewTypeEnum, warshipSo);
            
            log.Info($"accountId = {accountId} tmpPlayerId = {tmpPlayerId} id = {entity.id.value}");
            return entity;
        }
    }
}