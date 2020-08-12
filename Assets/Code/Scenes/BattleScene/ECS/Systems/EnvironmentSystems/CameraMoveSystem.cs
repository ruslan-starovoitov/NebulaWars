using Code.Common.Logger;
using Code.Common.Storages;
using Entitas;
using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.EnvironmentSystems
{
    public class CameraMoveSystem : IExecuteSystem
    {
        private readonly Camera mainCamera;
        private readonly GameContext gameContext;
        private Vector3 cameraVelocity = Vector3.zero;
        private readonly ILog log = LogManager.CreateLogger(typeof(CameraMoveSystem));

        public CameraMoveSystem(Contexts contexts, Camera camera)
        {
            gameContext = contexts.game;
            mainCamera = camera;
        }

        public void Execute()
        {
            var playerEntity = gameContext.GetEntityWithId(PlayerIdStorage.PlayerEntityId);
            if (playerEntity == null)
            {
                log.Debug("playerEntity is null");
                return;
            }
            
            if (!playerEntity.hasTransform)
            {
                log.Debug("playerEntity dont have transform");
                return;
            }


            Vector3 playerPosition = playerEntity.transform.position;
            Transform currentCameraPosition = mainCamera.transform;
            log.Debug($"playerPosition x {playerPosition.x} y {playerPosition.y}");
            // var tmpPosition = currentCameraPosition.position;
            Vector3 nextCameraPosition = new Vector3(playerPosition.x, 0, playerPosition.y) + new Vector3(0, 55, -37);
            // Vector3 newPosition = Vector3.SmoothDamp(tmpPosition, nextCameraPosition, ref cameraVelocity, 0.3f);
            currentCameraPosition.position = nextCameraPosition;
            mainCamera.transform.LookAt(playerPosition);
        }
    }
}