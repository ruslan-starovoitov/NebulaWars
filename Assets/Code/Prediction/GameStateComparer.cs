using System;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Code.Prediction
{
    /// <summary>
    /// При получении нового тика от игрового сервера проверяет, что аватар игрока был правильно предсказан
    /// </summary>
    public class GameStateComparer
    {
        /// <summary>
        /// Сравнивает предсказанное игровое состояние и настоящее состояние с сервера для сущности игрока
        /// </summary>
        /// <param name="gameState1"></param>
        /// <param name="gameState2"></param>
        /// <param name="avatarId"></param>
        /// <returns></returns>
        public bool IsSame(GameState gameState1, GameState gameState2, ushort avatarId)
        {
            if (gameState1 == null)
            {
                return false;
            }
            
            if (gameState2 == null)
            {
                return false;
            }
            
            if(!gameState1.transforms.TryGetValue(avatarId, out ViewTransform viewTransform1))
            {
                return false;
            }
            
            if(!gameState2.transforms.TryGetValue(avatarId, out ViewTransform viewTransform2))
            {
                return false;
            }


            if (Math.Abs(gameState1.tickMatchTimeSec - gameState2.tickMatchTimeSec) > 0.001f)
            {
                return false;
            }


            //сравнение позиции и поворота
            if (viewTransform1 != viewTransform2)
            {
                return false;
            }
        
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
            
            //todo сравнеение типа оружия
            // if (entity1.ChangeWeapon != entity2.ChangeWeapon)
            //     return false;
        
            return true;
        }
    }
}