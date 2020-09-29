using System;

namespace Plugins.submodules.SharedCode.Systems.Hits
{
    public class WarshipHitHandler
    {
        public void Hit(ServerGameEntity damageEntity, ServerGameEntity warshipEntity)
        {
            ushort parentId = damageEntity.parentWarship.entity.id.value;
            if (parentId == warshipEntity.id.value)
            {
                throw new Exception("Попадание по родителю не должно считаться");
            }
                
            //Отнять хп 
            float actualHealthPoints = warshipEntity.healthPoints.value - damageEntity.damage.value;
            warshipEntity.ReplaceHealthPoints(actualHealthPoints);
            
            //Пометить, если убит
            if (actualHealthPoints <= 0 && !warshipEntity.hasKilledBy)
            {
                int killerAccountId = damageEntity.parentWarship.entity.account.accountId;
                warshipEntity.AddKilledBy(killerAccountId);
            }
                
            //Уничтожить снаряд
            damageEntity.isDestroyed = true;
        }
    }
}