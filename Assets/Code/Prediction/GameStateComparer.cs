namespace Code.Prediction
{
    public class GameStateComparer
    {
        /// <summary>
        /// Сравнивает предсказанное игровое состояние и настоящее состояние с сервера для сущности игрока
        /// </summary>
        /// <param name="avatarId"></param>
        /// <returns></returns>
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
}