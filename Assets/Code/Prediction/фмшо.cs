using System;

namespace Code.Prediction
{
    public class GameStateBuffer
    {
        //todo начинать симуляцию после того, как буффер заполнится на три кадра
    }
    public class CyclicBuffer
    {
        //todo сделать циклический буффер на 1 сек
        //todo менять размер буффера по дисперсии 
        public class CyclicBufferElement
        {
            public object userInput;
            public object predictedUserTransform;
        }
    }
    public class GameStateHistory
    {
        public GameState Get(int tick)
        {
            throw new NotImplementedException();
        }

        public GameState Put(GameState tempState)
        {
            throw new NotImplementedException();
        }
    }

    public class GameStateComparer
    {
        public bool IsSame(GameState s1, GameState s2, uint avatarId)
        {
            if (s1 == null && s2 != null ||  s1 != null && s2 == null)
                return false;

            if (s1 == null && s2 == null)
                return false;

            var entity1 = s1.WorldState[avatarId];
            var entity2 = s2.WorldState[avatarId];

            if (entity1 == null && entity2 == null)
                return false;

            if (entity1 == null || entity2 == null)
                return false;

            if (s1.Time != s2.Time)
                return false;
        
            //todo сравнение позиций
            // if (s1.WorldState.Transform[avatarId] != s2.WorldState.Transform[avatarId])
            //     return false;
        
            //todo сравнение состояний оружия
            // foreach (var s1Weapon in s1.WorldState.Weapon)
            // {
            //     if (s1Weapon.Value.Owner.Id != avatarId)
            //         continue;
            //
            //     var s2Weapon = s2.WorldState.Weapon[s1Weapon.Key];
            //     if (s1Weapon.Value != s2Weapon)
            //         return false;
            //
            //     var s1Ammo = s1.WorldState.WeaponAmmo[s1Weapon.Key];
            //     var s2Ammo = s2.WorldState.WeaponAmmo[s1Weapon.Key];
            //     if (s1Ammo != s2Ammo)
            //         return false;
            //
            //     var s1Reload = s1.WorldState.WeaponReloading[s1Weapon.Key];
            //     var s2Reload = s2.WorldState.WeaponReloading[s1Weapon.Key];
            //     if (s1Reload != s2Reload)
            //         return false;
            // }

            //todo сравнеение угла поворота
            // if (entity1.Aiming != entity2.Aiming)
            //     return false;

            //todo сравнеение типа оружия
            // if (entity1.ChangeWeapon != entity2.ChangeWeapon)
            //     return false;
        
            return true;
        }
    }

    public class GameStateCopier
    {
        public void CopyPlayerEntities(GameState currentState, GameState tempState, uint playerId)
        {
            throw new NotImplementedException();
        }
    }

    public class Prediction
    {
        public GameState Predict(GameState last)
        {
            throw new NotImplementedException();
        }
    }
    public class PredictionManager
    {
        private GameState tempState;
        private Prediction prediction;
        private GameStateCopier gameStateCopier;
        private GameStateHistory localStateHistory;
        private GameStateComparer gameStateComparer;
        
        public GameState Reconcile(int currentTick, ServerGameStateData serverStateData,   GameState currentState, uint playerID)
        {
            GameState serverState =  serverStateData.GameState;
            var serverTick = serverState.Time;
            
            
            GameState predictedState = localStateHistory.Get(serverTick);

            //if predicted state matches server last state use server predicted state with predicted player
            if (gameStateComparer.IsSame(predictedState, serverState, playerID))
            {
                tempState.Copy(serverState);
                gameStateCopier.CopyPlayerEntities(currentState, tempState, playerID);
                return localStateHistory.Put(tempState); // replace predicted state with correct server state
            }

            //if predicted state doesn't match server state, reapply local inputs to server state
            var last = localStateHistory.Put(serverState); // replace wrong predicted state with correct server state
            for (var i = serverTick; i < currentTick; i++) 
            {
                last = prediction.Predict(last); // resimulate all wrong states
            }
            return last;
        }
    }

    public class ServerGameStateData
    {
        public GameState GameState { get; set; }
    }

    public class GameState
    {
        public int Time { get; set; }
        public object[] WorldState { get; set; }

        public void Copy(GameState serverState)
        {
            throw new NotImplementedException();
        }
    }
}