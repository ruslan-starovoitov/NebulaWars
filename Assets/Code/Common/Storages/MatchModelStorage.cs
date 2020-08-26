using System;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Common.Storages
{
    public class MatchModelStorage
    {
        private static readonly Lazy<MatchModelStorage> lazy = new Lazy<MatchModelStorage>(() => new MatchModelStorage());
        public static MatchModelStorage Instance => lazy.Value;
        private BattleRoyaleClientMatchModel matchData;
        private readonly ILog log = LogManager.CreateLogger(typeof(MatchModelStorage));

        public void SetMatchData(BattleRoyaleClientMatchModel matchModel)
        {
            matchData = matchModel;
        }

        public BattleRoyaleClientMatchModel GetMatchModel()
        {
            return matchData;
        }
    }
}