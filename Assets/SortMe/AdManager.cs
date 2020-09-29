// using GoogleMobileAds.Api;
// using UnityEngine;
//
// public class AdManager : MonoBehaviour
// {
//     private BannerView bannerView;
//     private InterstitialAd interstitialAd;
//     private RewardBasedVideoAd rewardBasedVideoAd;
//         
//     private void Start()
//     {
//         RequestBanner();
//         RequestInterstitial();
//         RequestVideoAd();
//     }
//
//     public void ShowBannerAd()
//     {
//         bannerView.Show();
//     }
//         
//     public void ShowInterstitialAd()
//     {
//         if (interstitialAd.IsLoaded())
//         {
//             interstitialAd.Show();
//         }
//     }
//         
//     public void ShowRewardedVideoAd()
//     {
//         if (rewardBasedVideoAd.IsLoaded())
//         {
//             rewardBasedVideoAd.Show();
//         }
//     }
//
//     private void RequestBanner()
//     {
//         bannerView = new BannerView(AdGlobals.BannerId, AdSize.SmartBanner, AdPosition.Bottom);
//             
//         //TODO remove AddTestDevice for release version
//         AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
//             
//         bannerView.LoadAd(adRequest);
//     } 
//         
//     private void RequestInterstitial()
//     {
//         interstitialAd = new InterstitialAd(AdGlobals.InterstitialId);
//             
//         //TODO remove AddTestDevice for release version
//         AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
//             
//         interstitialAd.LoadAd(adRequest);
//     }
//         
//     private void RequestVideoAd()
//     {
//         rewardBasedVideoAd = RewardBasedVideoAd.Instance;
//             
//         //TODO remove AddTestDevice for release version
//         AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();
//             
//         rewardBasedVideoAd.LoadAd(adRequest, AdGlobals.VideoId);
//     }
// }