﻿using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
 using SharedSimulationCode.Systems;
using UnityEngine;

namespace Plugins.submodules.SharedCode.Systems
{
    /// <summary>
    /// Создаёт сущности по вводу игроков.
    /// </summary>
    public class InputReceiver
    {
        private readonly Contexts contexts;
        private readonly InputMessagesMetaHistory messagesMetaHistory;

        public InputReceiver(Contexts contexts, InputMessagesMetaHistory messagesMetaHistory)
        {
            this.contexts = contexts;
            this.messagesMetaHistory = messagesMetaHistory;
        }

        public void AddMessage(ushort playerTmpId, uint inputMessageId, InputMessageModel inputMessageModel)
        {
            //Если сообщение уже обработано, то оно игнорируется
            if (messagesMetaHistory.TryAddId(playerTmpId, inputMessageId))
            {
                AddMovement(playerTmpId, inputMessageModel.GetVector2(), inputMessageModel.TickNumber);
                if (!float.IsNaN(inputMessageModel.Angle))
                {
                    AddAttack(playerTmpId,inputMessageModel.Angle , inputMessageModel.TickNumber);
                }
                
                if (inputMessageModel.UseAbility)
                {
                    AddAbility(playerTmpId, inputMessageModel.TickNumber);
                }    
            }
        }
        
        private void AddMovement(ushort playerId, Vector2 vector2, int tickNumber)
        {
            var inputEntity = contexts.serverInput.CreateEntity();
            inputEntity.AddPlayerInput(playerId);
            inputEntity.ReplaceMovement(vector2);
        }
        
        private void AddAttack(ushort playerId, float attackAngle, int tickNumber)
        {
            var inputEntity = contexts.serverInput.CreateEntity();
            inputEntity.AddPlayerInput(playerId);
            inputEntity.ReplaceAttack(attackAngle);
            inputEntity.ReplaceCreationTickNumber(tickNumber);
        }
        
        private void AddAbility(ushort playerId, int tickNumber)
        {    
            var inputEntity = contexts.serverInput.CreateEntity();
            inputEntity.AddPlayerInput(playerId);
            inputEntity.isTryingToUseAbility = true;
        }
        
        public void AddExit(ushort playerId)
        {
            var inputEntity = contexts.serverInput.CreateEntity();
            inputEntity.AddPlayerInput(playerId);
            inputEntity.isLeftTheGame = true;
        }
    }
}