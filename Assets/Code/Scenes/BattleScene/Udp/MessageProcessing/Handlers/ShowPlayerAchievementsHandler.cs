
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.ServerToPlayer.BattleStatus;

namespace Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers
{
    public class ShowPlayerAchievementsHandler : MessageHandler<ShowPlayerAchievementsMessage>
    {
        private readonly int matchId;
        private readonly ILog log = LogManager.CreateLogger(typeof(ShowPlayerAchievementsHandler));

        public ShowPlayerAchievementsHandler(int matchId)
        {
            this.matchId = matchId;
        }

        protected override void Handle(in ShowPlayerAchievementsMessage message, uint messageId, bool needResponse)
        {
            log.Debug($"Показать результаты боя игрока {nameof(message.MatchId)} {message.MatchId}");

            // Task.Run(async () =>
            // {
            //     await Task.Delay(5000);
            //     UnityThread.Execute(() => MatchRewardUiController.Instance().ShowPlayerAchievements());
            // });
            
            // if (matchId == message.MatchId)
            // {
            //     
            // }
            // else
            // {
            //     log.Error($"не тот матч!");
            // }
        }
    }
}