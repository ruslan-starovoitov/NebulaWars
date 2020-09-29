// #define ONE_PLAYER

using Entitas;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;

namespace Plugins.submodules.SharedCode.Systems.MapInitialization
{
    public class MapInitializeSystem:IInitializeSystem
    {
        private readonly BattleRoyaleMatchModel matchModel;
        private readonly WarshipsSpawnerBuilder warshipsSpawnerBuilder;
        private readonly AsteroidsSpawnerBuilder asteroidsSpawnerBuilder;
        private readonly ILog log = LogManager.CreateLogger(typeof(MapInitializeSystem));

        public MapInitializeSystem(Contexts contexts, BattleRoyaleMatchModel matchModel,
            WarshipsCharacteristicsStorage warshipsCharacteristicsStorage)
        {
            this.matchModel = matchModel;
            var warshipEntityFactory = new WarshipEntityFactory(contexts);
            warshipsSpawnerBuilder = new WarshipsSpawnerBuilder(new WarshipSpawnerHelper(warshipEntityFactory,
                warshipsCharacteristicsStorage));
            asteroidsSpawnerBuilder = new AsteroidsSpawnerBuilder(new AsteroidSpawnerHelper(contexts));
        }
        
        public void Initialize()
        {
            asteroidsSpawnerBuilder.SpawnAsteroids();
            warshipsSpawnerBuilder.SpawnPlayers(matchModel);
        }
    }
}