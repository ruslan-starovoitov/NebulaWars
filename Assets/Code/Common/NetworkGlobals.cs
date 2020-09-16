// #define LocalTesting
#if !UNITY_EDITOR
#undef LocalTesting
#endif

namespace Code.Common
{
    /// <summary>
    /// Хранит ссылки для доступа к матчмейкеру и профиль-серверу
    /// </summary>
    public static class NetworkGlobals
    {
        
#if  LocalTesting
        public const string GameServerIp = "127.0.0.1";
        private const string GameMatcherUrl = "http://127.0.0.1:53846";
#else
        public const string GameServerIp = "20.52.45.72";
        // private const string GameMatcherUrl = "https://tikaytech.games:53847";
        private const string GameMatcherUrl = "https://tikaytech.games";
#endif

        public static readonly string GetMatchDataUrl =  $"{GameMatcherUrl}/Player/GetMatchData";
        public static readonly string DeleteFromQueueUrl =  $"{GameMatcherUrl}/Player/DeleteFromQueue";
        public static readonly string ExitFromBattleUrl =  $"{GameMatcherUrl}/Player/ExitFromBattle";
        
        public static readonly string InitializeLobbyUrl =  $"{GameMatcherUrl}/LobbyModel/Create";
        public static readonly string ChangeUsernameUrl =  $"{GameMatcherUrl}/LobbyModel/SetUsername";
        public static readonly string ChangeSkinUrl =  $"{GameMatcherUrl}/Skin/Change";
        public static readonly string TestUrl =  $"{GameMatcherUrl}/Init/Resource";
        
       
        public static readonly string GetLootboxDataUrl =  $"{GameMatcherUrl}/Lootbox/BuyLootbox";
        
        public static readonly string ValidatePurchaseUrl =  $"{GameMatcherUrl}/Purchases/Validate";
        public static readonly string MarkOrderAsCompletedUrl =  $"{GameMatcherUrl}/Purchases/MarkOrderAsCompleted";
        public static readonly string GetMatchResultUrl =  $"{GameMatcherUrl}/MatchResult/Get";
        
        public static readonly string GetShopModel =  $"{GameMatcherUrl}/Shop/GetShopModel";
        public static readonly string BuyProduct =  $"{GameMatcherUrl}/Shop/BuyProduct";
        public static readonly string WarshipImprovementPurchasingUrl =  
            $"{GameMatcherUrl}/WarshipImprovementPurchasing/BuyImprovement";

        
    }
}