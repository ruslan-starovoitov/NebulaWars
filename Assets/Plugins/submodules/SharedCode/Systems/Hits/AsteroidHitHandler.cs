using System;
using Plugins.submodules.SharedCode.Logger;

namespace Plugins.submodules.SharedCode.Systems.Hits
{
    public class AsteroidHitHandler
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(AsteroidHitHandler));
        
        public void Hit(ServerGameEntity damageEntity, ServerGameEntity withHp)
        {
            ushort parentId = damageEntity.parentWarship.entity.id.value;
            if (parentId == withHp.id.value)
            {
                throw new Exception("Попадание по родителю не должно считаться");
            }
            
            //Отнять хп 
            float actualHealthPoints = withHp.healthPoints.value - damageEntity.damage.value;
            withHp.ReplaceHealthPoints(actualHealthPoints);
            
            //Уничтожить снаряд
            damageEntity.isDestroyed = true;
        }
    }
}