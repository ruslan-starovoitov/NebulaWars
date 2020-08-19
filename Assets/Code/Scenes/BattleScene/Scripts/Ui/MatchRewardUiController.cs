using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Code.Common;
using Code.Common.Statistics;
using Code.Common.Storages;
using Code.Scenes.BattleScene.ECS;
using Code.Scenes.BattleScene.Experimental;
using Code.Scenes.BattleScene.Scripts.Ui;
using Code.Scenes.BattleScene.Udp.MessageProcessing.Handlers;
using Libraries.NetworkLibrary.Experimental;
using NetworkLibrary.NetworkLibrary.Http;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.Scripts
{
    /// <summary>
    /// Отвечает за анимацию окна послебоевых наград.
    /// </summary>
    [RequireComponent(typeof(LobbyLoaderController))]
    [RequireComponent(typeof(BattleUiController))]
    public class MatchRewardUiController : Singleton<MatchRewardUiController>
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject canvasCameraSpace;
        
        [SerializeField] private GameObject leftMenu;
        [SerializeField] private Text battleRatingDelta;
        [SerializeField] private Image warshipImage;
        [SerializeField] private Text spaceshipNameText;
        
        [SerializeField] private GameObject rightMenu;
        [SerializeField] private GameObject spaceshipRatingScaleRoot;
        [SerializeField] private Text spaceshipRankValueText;
        [SerializeField] private GameObject spaceshipRatingScaleText;
        [SerializeField] private GameObject spaceshipRatingScaleTrophy;
        [SerializeField] private Slider spaceshipRatingScaleSlider;
        
        
        [SerializeField] private GameObject lootboxRewardSectionRoot;
        
        
        [SerializeField] private GameObject totalSumRoot;
        [SerializeField] private GameObject coinsTotalSumText;
        [SerializeField] private GameObject moneyIconGo;
        
        [SerializeField] private GameObject exitButton;

        private bool isAnimationStarted;
        private ClientMatchSimulation clientMatchSimulation;
        private BattleUiController battleUiController;
        private LobbyLoaderController lobbyLoaderController;
        private readonly ILog log = LogManager.CreateLogger(typeof(MatchRewardUiController));
        
        private void Start()
        {
            //Выключить канвас на котором расположено окно наград
            canvasCameraSpace.SetActive(false);
            battleUiController = GetComponent<BattleUiController>();
            lobbyLoaderController = GetComponent<LobbyLoaderController>();
            clientMatchSimulation = GetComponent<ClientMatchSimulation>();
        }

        public void ShowPlayerAchievements()
        {
            if (isAnimationStarted)
            {
                return;
            }

            isAnimationStarted = true;
            clientMatchSimulation.SelfDestruct();
            
            StartCoroutine(ShowPlayerAchievementsCoroutine());
        }
        
        public void Button_Exit_OnClick()
        {
            lobbyLoaderController.LoadLobbyScene();            
        }
        
        private IEnumerator ShowPlayerAchievementsCoroutine()
        {
            //Достать данные из глобальных классов
            BattleRoyaleClientMatchModel matchModel = MatchModelStorage.Instance.GetMatchModel();
            int matchId = matchModel.MatchId;
            if (PlayerIdStorage.TryGetServiceId(out string playerServiceId))
            {
                if (playerServiceId == null)
                {
                    throw new Exception($"{nameof(playerServiceId)} is null");
                }
                //Загрузить данные с профиль-сервера
                Task<MatchResultDto> task = ExperimentalDich.GetMatchReward(matchId, playerServiceId);
                yield return new WaitUntil(()=>task.IsCompleted);
                MatchResultDto matchResultDto = task.Result;
            
                
                //Если не загрузилось, перейти в лобби
                if (matchResultDto == null||task.IsFaulted||task.IsCanceled)
                {
                    if (matchResultDto == null)
                    {
                        log.Error($"matchResultDto is null");
                    }
                    else
                    {
                        PlayerAchievementsUtils.LogPlayerAchievements(matchResultDto);
                    }

                    if (task.IsFaulted)
                    {
                        log.Error($"task.IsFaulted");
                    }
                    
                    if (task.IsCanceled)
                    {
                        log.Error($"task.IsCanceled");
                    }
                    
                    log.Error("Не удалось загрузить результат боя игрока.");
                    lobbyLoaderController.LoadLobbyScene();
                }

                PlayerAchievementsUtils.LogPlayerAchievements(matchResultDto);
                
                //Показать анимацию
                ShowPlayerAchievements(matchResultDto);
            }
            else
            {
                log.Error($"{nameof(ShowPlayerAchievementsCoroutine)} {nameof(playerServiceId)} is null");
            }
        }

        private void ShowPlayerAchievements(MatchResultDto matchResultDto)
        {
            log.Info(nameof(ShowPlayerAchievements) + " start");
            mainCamera.transform.position = Vector3.zero;
            battleUiController.DisableZoneAndOverlayCanvas();
            canvasCameraSpace.SetActive(true);
            ShowAchievementsWithAnimationAsync(matchResultDto).ConfigureAwait(true);
            log.Info(nameof(ShowPlayerAchievements) + " end");
        }

        private async Task ShowAchievementsWithAnimationAsync(MatchResultDto matchResultDto)
        {
            try
            {
                DisableMenuElements();
                ShowWarship(matchResultDto.SkinName);
                SetBattleRatingDelta(matchResultDto.MatchRatingDelta);
                canvasCameraSpace.SetActive(true);
                await ShowAnimationOfAppearanceSmoothly(leftMenu);
                await ShowAnimationOfAppearanceSmoothly(rightMenu);
                ShowSpaceshipRatingGo();
                await ShowRatingCalculation(matchResultDto.CurrentWarshipRating, matchResultDto.MatchRatingDelta);
                await ShowRewards(matchResultDto.LootboxPoints);

                int sum = matchResultDto.LootboxPoints.Values.Sum();
                await ShowMoneyAccrual(0, sum);
                ShowExitButton();
            }
            catch (Exception e)
            {
                log.Fatal("Ui throw an exception ."+e.Message);   
            }
        }
       
        private void ShowWarship(string spaceshipPrefabName)
        {
            ShowSpaceshipPrefab(spaceshipPrefabName);
            SetSpaceshipName(spaceshipPrefabName);
        }

        private void ShowSpaceshipPrefab(string spaceshipPrefabName)
        {
            warshipImage.sprite = Resources.Load<Sprite>($"SkinPreview/{spaceshipPrefabName}");
        }

        private void SetSpaceshipName(string spaceshipName)
        {
            spaceshipNameText.text = spaceshipName;
        }
        
        private void SetBattleRatingDelta(int ratingDelta)
        {
            if (ratingDelta > 0)
            {
                battleRatingDelta.text = "+" + ratingDelta;
            }
            else
            {
                battleRatingDelta.text = ratingDelta.ToString();
            }
        }
        
        private void DisableMenuElements()
        {
            leftMenu.SetActive(false);
            rightMenu.SetActive(false);
            spaceshipRatingScaleRoot.SetActive(false);
            exitButton.SetActive(false);
            totalSumRoot.SetActive(false);
            lootboxRewardSectionRoot.SetActive(false);
        }
        
        private async Task ShowAnimationOfAppearanceSmoothly(GameObject imageGo)
        {
            var rectTransform = imageGo.GetComponent<Image>().rectTransform;
            var originalScale = rectTransform.localScale;
            var smallestScale = originalScale * 0.7f;
            var maximumScale = originalScale * 1.2f;

            const int increaseDelayMs = 10;
            const int reductionDelayMs = 7;
            
            //Умешьшить картинку до минимального размера
            rectTransform.localScale = smallestScale;
            //Показать её
            imageGo.SetActive(true);
            //увеличиваем картинку до максимального размера
            while (rectTransform.localScale.sqrMagnitude <  maximumScale.sqrMagnitude)
            {
                rectTransform.localScale += originalScale * 0.03f;
                await Task.Delay(increaseDelayMs);
            }
            //уменьшаем картинку до нормального размера
            while (rectTransform.localScale.sqrMagnitude >  originalScale.sqrMagnitude)
            {
                rectTransform.localScale -= originalScale * 0.05f;
                await Task.Delay(reductionDelayMs);
            }
        }
        
        private void ShowSpaceshipRatingGo()
        {
            spaceshipRatingScaleRoot.SetActive(true);
        }
        
        private async Task ShowRatingCalculation(int currentWarshipRating, int deltaRating)
        {
            CancellationTokenSource cancellationTokenSourceForTrophy = new CancellationTokenSource();
            StartTrophyAnimationAsync(cancellationTokenSourceForTrophy.Token);
            
            Text textObj = spaceshipRatingScaleText.GetComponent<Text>();
            spaceshipRatingScaleText.SetActive(true);
            
            int frameRatingDelta = deltaRating >= 0 ? 1 : -1;
            int startRating = currentWarshipRating-deltaRating;
            int finishValue = currentWarshipRating;
            int currentRank = 0;

            while (startRating != finishValue+frameRatingDelta)
            {
                WarshipRankModel warshipRankModel = WarshipRatingScaleStorage.Instance.GetRankModel(startRating);
                textObj.text = warshipRankModel.ToString();

                spaceshipRatingScaleSlider.value = warshipRankModel.rankProgress;

                if (currentRank != warshipRankModel.currentRank)
                {
                    spaceshipRankValueText.text = warshipRankModel.currentRank.ToString();
                    currentRank = warshipRankModel.currentRank;
                }

                startRating += frameRatingDelta;
                // ReSharper disable once MethodSupportsCancellation
                await Task.Delay(20);
            }
           
            cancellationTokenSourceForTrophy.Cancel();
        }
        
        private void StartTrophyAnimationAsync(CancellationToken cancellationToken)
        {
            Image image = spaceshipRatingScaleTrophy.GetComponent<Image>();
#pragma warning disable 4014
            StartIconAnimationAsync(image, cancellationToken, 0.25f, 10);
#pragma warning restore 4014
        }
        
        private async Task StartIconAnimationAsync(Image image , CancellationToken cancellationToken, float deltaY, 
            int liftingDelayMs)
        {
            //TODO тут нужен try? я ведь не жду завершения этого метода
            try
            {
                var originalPosition = image.rectTransform.position;
                while (!cancellationToken.IsCancellationRequested)
                {
                    int numberOfSteps = 5;
                    //Поднять кубок вверх
                    for (int stepNumber = 0; stepNumber <= numberOfSteps; stepNumber++)
                    {
                        image.rectTransform.position = originalPosition + Vector3.up * (deltaY * stepNumber) / numberOfSteps;
                        // ReSharper disable once MethodSupportsCancellation
                        await Task.Delay(liftingDelayMs);
                    }
                    //Опустить на исходную высоту
                    for (int stepNumber = numberOfSteps; stepNumber >= 0 ; stepNumber--)
                    {
                        image.rectTransform.position = originalPosition + Vector3.up * (deltaY * stepNumber) / numberOfSteps;
                        // ReSharper disable once MethodSupportsCancellation
                        await Task.Delay(liftingDelayMs);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error($"Брошено исключение в {nameof(StartIconAnimationAsync)}. {e.Message}");
            }
        }
        
        private async Task ShowMoneyAccrual(int startValue, int finishValue)
        {
            totalSumRoot.SetActive(true);
            CancellationTokenSource cancellationTokenSourceForMoneyIcon = new CancellationTokenSource();
            StartMoneyIconAnimationAsync(cancellationTokenSourceForMoneyIcon.Token);
            Text textObj = coinsTotalSumText.GetComponent<Text>();
            int currentValue = startValue;
            int delta = 1;
            while (currentValue!=finishValue)
            {
                currentValue += delta;
                textObj.text = currentValue.ToString();
                // ReSharper disable once MethodSupportsCancellation
                await Task.Delay(40);
            }
            cancellationTokenSourceForMoneyIcon.Cancel();
        }
        
        private void StartMoneyIconAnimationAsync(CancellationToken cancellationToken)
        {
            Image image = moneyIconGo.GetComponent<Image>();
#pragma warning disable 4014
            StartIconAnimationAsync(image, cancellationToken, 0.25f, 10);
#pragma warning restore 4014
        }
        
        private void ShowExitButton()
        {
            exitButton.SetActive(true);
        }
        
        private async Task ShowRewards(Dictionary<MatchRewardTypeEnum, int> lootboxRewards)
        {
            lootboxRewardSectionRoot.SetActive(true);
            lootboxRewardSectionRoot.transform.DestroyAllChildren();
            foreach (var pair in lootboxRewards)
            {
                int rewardAmount = pair.Value;
                
                //Создать префаб 
                GameObject prefab = Resources.Load<GameObject>("Prefabs/MatchReward/Empty_LootboxReward");
                GameObject lootboxReward = Instantiate(prefab, lootboxRewardSectionRoot.transform, false);
                
                //Установить название
                Text rewardNameText = lootboxReward.transform.Find("Text_Name").GetComponent<Text>();
                rewardNameText.text = RewardNameTranslator.Translate(pair.Key);
                
                //Достать текст для суммы
                Text amountText = lootboxReward.transform.Find("Image_LootboxReward/Text_Sum").GetComponent<Text>();
                
                //Показать начисление
                await IncreaseTheNumber(amountText, 0, rewardAmount, 10);    
            }
        }
        
        private async Task IncreaseTheNumber(Text text, int startValue, int finishValue, int delayMs)
        {
            int currentValue = startValue;
            text.text = "0";
            int delta = finishValue > startValue ? 1 : -1;
            while (currentValue!=finishValue)
            {
                currentValue += delta;
                text.text = currentValue.ToString();
                await Task.Delay(delayMs);
            }
        }
    }
}