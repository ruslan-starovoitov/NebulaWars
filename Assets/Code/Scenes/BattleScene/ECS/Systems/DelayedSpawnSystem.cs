﻿using Code.Scenes.BattleScene.ECS.Components.Game.TimerComponents;
using Code.Scenes.BattleScene.ScriptableObjects;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems
{
    public class DelayedSpawnSystem : BaseTimerSubtractionSystem<DelayedSpawnComponent>
    {
        private readonly GameContext gameContext;
        protected override int PredictedCapacity { get; } = 64;

        public DelayedSpawnSystem(Contexts contexts) : base(contexts)
        {
            gameContext = contexts.game;
        }

        protected override void OnTimeExpired(GameEntity entity)
        {
            var spawn = entity.delayedSpawn;
            var obj = ViewObjectsBase.Instance.GetViewObject(spawn.typeId);
            obj.FillEntity(gameContext, entity);
            entity.ReplacePosition(new Vector3(spawn.positionX, spawn.positionY, -0.00001f * entity.id.value));
            entity.ReplaceDirection(spawn.direction);
            base.OnTimeExpired(entity);
        }
    }
}