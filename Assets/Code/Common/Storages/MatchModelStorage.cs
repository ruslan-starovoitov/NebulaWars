using System;
using NetworkLibrary.NetworkLibrary.Http;

namespace Code.Common.Storages
{
    public class MatchModelStorage
    {
        private static readonly Lazy<MatchModelStorage> lazy = new Lazy<MatchModelStorage>(() => new MatchModelStorage());
        public static MatchModelStorage Instance => lazy.Value;
        private BattleRoyaleClientMatchModel matchData;

        public void SetMatchData(BattleRoyaleClientMatchModel matchDataArg)
        {
            matchData = matchDataArg;
        }

        public BattleRoyaleClientMatchModel GetMatchModel()
        {
            return matchData;
        }
    }
}