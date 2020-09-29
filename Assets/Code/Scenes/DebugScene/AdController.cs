// using UnityEngine;
// using GoogleMobileAds.Api;
// namespace Code.Scenes.DebugScene
// {
//     public class AdController : MonoBehaviour
//     {
//         private BannerView bannerView;
//         private InterstitialAd interstitialAd;
//         private RewardBasedVideoAd rewardBasedVideoAd;
//         
//         private void Start()
//         {
//             // MobileAds.SpawnSection(AdGlobals.AppId);
//
//             // RequestBanner();
//             // RequestInterstitial();
//             // RequestVideoAd();
//         }
//
//         public void ShowBannerAd()
//         {
//             bannerView.Show();
//         }
//         
//         public void ShowInterstitialAd()
//         {
//             if (interstitialAd.IsLoaded())
//             {
//                 interstitialAd.Show();
//             }
//         }
//         
//         public void ShowRewardedVideoAd()
//         {
//             if (rewardBasedVideoAd.IsLoaded())
//             {
//                 rewardBasedVideoAd.Show();
//             }
//         }
//
//         private void RequestBanner()
//         {
//             bannerView = new BannerView(Common.AdGlobals.BannerId, AdSize.SmartBanner, AdPosition.Bottom);
//             
//             //TODO remove AddTestDevice for release version
//             AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
//             
//             bannerView.LoadAd(adRequest);
//         } 
//         
//         private void RequestInterstitial()
//         {
//             interstitialAd = new InterstitialAd(Common.AdGlobals.InterstitialId);
//             
//             //TODO remove AddTestDevice for release version
//             AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
//             
//             interstitialAd.LoadAd(adRequest);
//         }
//         
//         private void RequestVideoAd()
//         {
//             rewardBasedVideoAd = RewardBasedVideoAd.Instance;
//             
//             //TODO remove AddTestDevice for release version
//             AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
//             
//             rewardBasedVideoAd.LoadAd(adRequest, Common.AdGlobals.VideoId);
//         }
//     }
// };