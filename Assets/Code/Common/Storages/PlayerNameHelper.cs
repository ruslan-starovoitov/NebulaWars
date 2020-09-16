using System.Text.RegularExpressions;
using NetworkLibrary.NetworkLibrary.Http;

namespace Code.Common.Storages
{
    public class PlayerNameHelper
    {
        private readonly BattleRoyaleClientMatchModel matchModel;

        public PlayerNameHelper(BattleRoyaleClientMatchModel matchModel)
        {
            this.matchModel = matchModel;
        }
        
        public string GetName(int playerId, ViewTypeEnum typeId)
        {
            string username = matchModel.GetUsername(playerId);
            if (username == null)
            {
                string viewTypeName = Regex.Replace(typeId.ToString("G"), @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))"," $0");
                return viewTypeName;
            }

            return username;
        }
    }
}