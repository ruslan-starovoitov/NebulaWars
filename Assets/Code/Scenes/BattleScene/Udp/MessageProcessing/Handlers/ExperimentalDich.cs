using System;
using System.Threading.Tasks;
using Code.Common;
using Libraries.NetworkLibrary.Experimental;
using Plugins.submodules.SharedCode.Logger;
using ZeroFormatter;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public static class ExperimentalDich
    {
        private static readonly ILog Log = LogManager.CreateLogger(typeof(ExperimentalDich));
        public static async Task<MatchResultDto> GetMatchReward(int matchId, string playerServiceId)
        {
            Log.Info($"{nameof(matchId)} {matchId} {nameof(playerServiceId)} {playerServiceId}"); 
            
            (string name, string value)[] query = 
            {
                (nameof(matchId), matchId.ToString()),
                (nameof(playerServiceId), playerServiceId)
            };

            for (int i = 0; i < 10; i++)
            {
                if (i != 0)
                {
                    await Task.Delay(500);
                }
                try
                {
                    byte[] data = await HttpWrapper.Get(NetworkGlobals.GetMatchResultUrl, query);    
                    var result = ZeroFormatterSerializer.Deserialize<MatchResultDto>(data);
                    if (result == null)
                    {
                        Log.Warn("Не удалось получить данные о результате боя");
                        continue;
                    }
                    return result;
                }
                catch (Exception e)
                {
                    Log.Warn(e.Message);
                }    
            }

            return null;
        }
    }
}