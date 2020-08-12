using System.Collections.Generic;

namespace Code.Scenes.BattleScene.ECS.Systems.NetworkSyncSystems
{
    public interface IPlayersStorage
    {
        void SetNewPlayers(Dictionary<int, ushort> newPlayers);
    }
}