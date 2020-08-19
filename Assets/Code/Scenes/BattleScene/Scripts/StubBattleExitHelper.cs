using System;
using System.Threading.Tasks;
using Code.Common;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Scripts
{
    public class StubBattleExitHelper
    {
        private readonly ILog log = LogManager.CreateLogger(typeof(StubBattleExitHelper));
        
        public async Task StubNotifyGameServerAsync(UdpSendUtils udpSendUtils)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    udpSendUtils.SendExitNotification();
                    await Task.Delay(100);
                }
            }
            catch (Exception e)
            {
                log.Error($"Брошено исключение в методе {nameof(StubNotifyGameServerAsync)}. {e.Message}");
            }
        }
    }
}