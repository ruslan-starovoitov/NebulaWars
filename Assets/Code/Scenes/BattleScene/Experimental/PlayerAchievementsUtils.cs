using Code.Common;
using Libraries.NetworkLibrary.Experimental;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.Experimental
{
    public static class PlayerAchievementsUtils
    {
        private static readonly ILog log = LogManager.CreateLogger(typeof(PlayerAchievementsUtils));
        
        public static void LogPlayerAchievements(MatchResultDto matchResultDto)
        {
            if (matchResultDto == null)
            {
                log.Info($"{nameof(matchResultDto)} is null");
                return;
            }
            log.Info($"{nameof(matchResultDto.SkinName)} {matchResultDto.SkinName}");
            log.Info($"{nameof(matchResultDto.MatchRatingDelta)} {matchResultDto.MatchRatingDelta}");
            log.Info($"{nameof(matchResultDto.CurrentWarshipRating)} {matchResultDto.CurrentWarshipRating}");
            if (matchResultDto.LootboxPoints.Count == 0)
            {
                log.Info("Список наград пуст");
            }
            
            foreach (var pair  in matchResultDto.LootboxPoints)
            {
                log.Info($"{pair.Key} {pair.Value}");
            }
        }
    }
}