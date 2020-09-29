using NetworkLibrary.NetworkLibrary.Http;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.MapInitialization
{
    /// <summary>
    /// Спавнить всех игроков на карте
    /// </summary>
    public class WarshipsSpawnerBuilder
    {
        private readonly WarshipSpawnerHelper warshipSpawnerHelper;

        public WarshipsSpawnerBuilder(WarshipSpawnerHelper warshipSpawnerHelper)
        {
            this.warshipSpawnerHelper = warshipSpawnerHelper;
        }
        
        public void SpawnPlayers(BattleRoyaleMatchModel matchModel)
        {
            Vector3 position = new Vector3();
#if ONE_PLAYER
            var firstPlayer = matchModel.GameUnits.Players.First();
            CreateWarship(position, firstPlayer.TemporaryId, firstPlayer.AccountId, ViewTypeEnum.StarSparrow1);
#else
            foreach (var player in matchModel.GameUnits.Players)
            {
                warshipSpawnerHelper.CreateWarship(position, player.TemporaryId, player.AccountId, 
                    ViewTypeEnum.StarSparrow1);
                position = position + new Vector3(15, 0,15);
            }
            
            foreach (var botModel in matchModel.GameUnits.Bots)
            {
                ServerGameEntity bot = warshipSpawnerHelper.CreateWarship(position, botModel.TemporaryId, -botModel.TemporaryId, 
                    ViewTypeEnum.StarSparrow1);
                bot.isBot = true;
                position = position + new Vector3(15, 0,15);
            }
#endif            
        }
    }
}