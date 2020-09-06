using System;
using Code.Common.Storages;
using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS
{
    /// <summary>
    /// Устанавливает скорость игрока в 0 перед предсказанием его позиции по вводу.
    /// Это нужно для того, чтобы предсказание игрока работало точно как и на сервере.
    /// </summary>
    public class PlayerStopSystem : IExecuteSystem
    {
        private readonly ServerGameContext gameContext;

        public PlayerStopSystem(Contexts contexts)
        {
            gameContext = contexts.serverGame;
        }

        public void Execute()
        {
            ushort playerEntityId = PlayerIdStorage.PlayerEntityId;
            if (playerEntityId == 0)
            {
                throw new Exception("Пустой playerEntityId");
            }
            
            ServerGameEntity playerEntity = gameContext.GetEntityWithId(playerEntityId);
            if (playerEntity.hasRigidbody)
            {
                playerEntity.rigidbody.value.velocity = Vector3.zero;
                playerEntity.rigidbody.value.angularVelocity = Vector3.zero;
            }
        }
    }
}