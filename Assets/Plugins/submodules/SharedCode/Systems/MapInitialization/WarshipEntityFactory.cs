using System;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems.MapInitialization
{
    /// <summary>
    /// Создаёт сущность корабля и заполняет её данными со scriptableObject
    /// </summary>
    public class WarshipEntityFactory
    {
        private readonly ServerGameContext gameContext;
        private readonly WarshipSoValidator warshipSoValidator;

        public WarshipEntityFactory(Contexts contexts)
        {
            gameContext = contexts.serverGame;
            warshipSoValidator = new WarshipSoValidator();
        }
        
        public ServerGameEntity Create(Vector3 position, ushort tmpPlayerId, int accountId, 
            ViewTypeEnum viewTypeEnum, WarshipSO warshipSo)
        {
            warshipSoValidator.Validate(warshipSo);
            
            ServerGameEntity entity = gameContext.CreateEntity();
            entity.AddPlayer(tmpPlayerId);
            entity.AddNickname("warship");
            entity.AddAccount(accountId);
            entity.AddMaxSpeed(warshipSo.maxVelocity);
            entity.AddAngularVelocity(warshipSo.angularVelocity);
            if (warshipSo.maxHealth <= 0)
            {
                throw new Exception($"Нельзя спавнить корабли таких хп {warshipSo.maxHealth}");
            }
            
            entity.AddHealthPoints(warshipSo.maxHealth);
            entity.AddMaxHealthPoints(warshipSo.maxHealth);
            entity.AddTeam((byte)(tmpPlayerId+1));
            entity.AddViewType(viewTypeEnum);
            entity.AddSpawnTransform(position, Quaternion.identity);

            return entity;
        }
    }
}