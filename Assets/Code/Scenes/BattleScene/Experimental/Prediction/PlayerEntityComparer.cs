using Plugins.submodules.SharedCode;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Prediction;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// При получении нового тика от игрового сервера проверяет, что аватар игрока был правильно предсказан
    /// </summary>
    public class PlayerEntityComparer
    {
        private readonly TransformComparer transformComparer = new TransformComparer();
        private readonly ILog log = LogManager.CreateLogger(typeof(PlayerEntityComparer));
        /// <summary>
        /// Сравнивает предсказанное игровое состояние и настоящее состояние с сервера для сущности игрока
        /// </summary>
        /// <param name="snapshot1"></param>
        /// <param name="snapshot2"></param>
        /// <param name="avatarId"></param>
        /// <returns></returns>
        public bool IsSame(Snapshot snapshot1, Snapshot snapshot2, ushort avatarId)
        {
            if (snapshot1 == null)
            {
                return false;
            }
            
            if (snapshot2 == null)
            {
                return false;
            }
            
            if(!snapshot1.transforms.TryGetValue(avatarId, out var viewTransform1))
            {
                return false;
            }
            
            if(!snapshot2.transforms.TryGetValue(avatarId, out var viewTransform2))
            {
                return false;
            }
            
            //сравнение позиции и поворота
            if (!transformComparer.IsSame(viewTransform1, viewTransform2))
            {
                log.Debug("Transform разные");
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