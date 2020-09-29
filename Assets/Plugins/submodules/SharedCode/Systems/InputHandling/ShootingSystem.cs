using Entitas;
using UnityEngine.Rendering;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    /// <summary>
    /// Создаёт сущности для пуль/снарядов обычной атаки по вводу игрока
    /// </summary>
    public class ShootingSystem : IExecuteSystem
    {
        private readonly ShootingHelper shootingHelper;
        private readonly ServerGameContext gameContext;
        private readonly IGroup<ServerInputEntity> attackGroup;

        public ShootingSystem(Contexts contexts)
        {
            var matcher = ServerInputMatcher
                .AllOf(ServerInputMatcher.Attack, ServerInputMatcher.PlayerInput);
            attackGroup = contexts.serverInput.GetGroup(matcher);
            gameContext = contexts.serverGame;
            shootingHelper = new ShootingHelper(contexts);
        }

        public void Execute()
        {
            foreach (var inputEntity in attackGroup)
            {
                ushort playerId = inputEntity.playerInput.playerEntityId;
                ServerGameEntity playerEntity = gameContext.GetEntityWithPlayer(playerId);

                shootingHelper.Shoot(playerEntity, inputEntity);
            }
        }
    }
}    