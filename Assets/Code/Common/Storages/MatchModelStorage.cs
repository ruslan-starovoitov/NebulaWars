using System;
using System.Linq;
using JetBrains.Annotations;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Common.Storages
{
    public interface IMatchModelStorage
    {
        BattleRoyaleClientMatchModel GetMatchModel();
    }
    
    public class MatchModelStorage:IMatchModelStorage
    {
        private BattleRoyaleClientMatchModel matchData;
        public static MatchModelStorage Instance => lazy.Value;
        private readonly ILog log = LogManager.CreateLogger(typeof(MatchModelStorage));
        private static readonly Lazy<MatchModelStorage> lazy = new Lazy<MatchModelStorage>(() => new MatchModelStorage());

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