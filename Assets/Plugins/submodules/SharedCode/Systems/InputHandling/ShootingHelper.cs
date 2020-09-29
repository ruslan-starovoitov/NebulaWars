using System.Collections.Generic;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.InputHandling
{
    public class ShootingHelper
    {
        private readonly ServerGameContext gameContext;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShootingHelper));
        
        public ShootingHelper(Contexts contexts)
        {
            gameContext = contexts.serverGame;
        }
        
        public void Shoot(ServerGameEntity playerEntity, ServerInputEntity inputEntity)
        {
            if (playerEntity.hasCannonCooldown)
            {
                return;
            }
            
            float attackStickDirection = inputEntity.attack.direction;
            if (float.IsNaN(attackStickDirection))
            {
                return;
            }
            
            if (!playerEntity.hasShootingPoints)
            {
                log.Error("Если есть Attack то должен быть ShootingPoints");
                return;
            }
            
            //выстрел
            Transform warshipTransform = playerEntity.transform.value;
            List<Transform> shootingPoints = playerEntity.shootingPoints.values;
            playerEntity.ReplaceCannonCooldown(0.5f);
            foreach (var shootingTransform in shootingPoints)
            {
                //спавн пуль
                var projectileEntity = gameContext.CreateEntity();
                projectileEntity.AddTickNumber(inputEntity.creationTickNumber.value);
                projectileEntity.AddDamage(100);
                projectileEntity.AddViewType(ViewTypeEnum.DefaultShoot);
                // projectileEntity.isSpawnProjectile = true;
                Vector3 position = shootingTransform.position;
                // Debug.LogError($"shootingPoint.position {position.x} {position.y} {position.z}");
                Vector3 spawnPosition = warshipTransform.localPosition + position;
                projectileEntity.AddSpawnTransform(position, shootingTransform.rotation);
                Vector3 direction = shootingTransform.transform.rotation *Vector3.forward;
                projectileEntity.AddSpawnForce(direction.normalized * 20f);
                projectileEntity.AddParentWarship(playerEntity);
            }
        }
    }
}