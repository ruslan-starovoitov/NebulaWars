using System.Collections.Generic;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.PositionMessages;

namespace Plugins.submodules.SharedCode.LagCompensation
{
    public class WithHpArrangeTransformSystem : ArrangeTransformSystem
    {
        private readonly IGroup<ServerGameEntity> withHpGroup;
        private readonly ILog log = LogManager.CreateLogger(typeof(WithHpArrangeTransformSystem));

        public WithHpArrangeTransformSystem(Contexts contexts)
        {
            withHpGroup = contexts.serverGame.GetGroup(ServerGameMatcher
                .AllOf(ServerGameMatcher.Transform, ServerGameMatcher.HealthPoints));
        }
        
        public override void Execute(Snapshot snapshot)
        {
            string message = $"Расстановка объектов по местам. Кол-во сущностей в контексте {withHpGroup.count}| " +
                             $"Кол-во сущностей в снимке {snapshot.transforms.Count}";
            // log.Debug(message);
            HashSet<ushort> needHandle = new HashSet<ushort>(snapshot.transforms.Keys);
            foreach (var withHp in withHpGroup)
            {
                var isActive = snapshot.transforms.ContainsKey(withHp.id.value);
                if (!isActive)
                {
                    //выключить если этот объект тогда не существовал
                    withHp.transform.value.gameObject.SetActive(false);
                }
                else
                {
                    needHandle.Remove(withHp.id.value);
                    //передвинуть если этот объект был
                    withHp.transform.value.gameObject.SetActive(true);
                    ViewTransformCompressed viewTransform = snapshot.transforms[withHp.id.value];
                    withHp.transform.value.Change(viewTransform);
                }
            }
            
            if (needHandle.Count != 0)
            {
                log.Info($"Не обработано {needHandle.Count} объектов из снимка");    
            }
        }
    }
}