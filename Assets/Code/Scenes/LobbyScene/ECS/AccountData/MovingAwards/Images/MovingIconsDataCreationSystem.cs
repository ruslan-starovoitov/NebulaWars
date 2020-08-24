using System;
using System.Collections.Generic;
using System.Linq;
using Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.Images.Experimental;
using Code.Scenes.LobbyScene.Scripts.UiStorages;
using Entitas;
using Plugins.submodules.EntitasCore.Entitas.Components;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;
using Random = System.Random;

namespace Code.Scenes.LobbyScene.ECS.AccountData.MovingAwards.Images
{
    /// <summary>
    /// Создаёт компоненты для наград. Внутри компонента указан его путь.
    /// </summary>
    public class MovingIconsDataCreationSystem:ReactiveSystem<LobbyUiEntity>
    {
        private readonly float screenHeight;
        
        private readonly Random random = new Random();
        private readonly LobbyUiContext contextsLobbyUi;
        
        private readonly ILog log = LogManager.CreateLogger(typeof(MovingIconsDataCreationSystem));
        private readonly MovingAwardTrajectoryFactory awardTrajectoryFactory;

        public MovingIconsDataCreationSystem(Contexts contexts, RectTransform movingAwardsParentRectTransform)
            :base(contexts.lobbyUi)
        {
            contextsLobbyUi = contexts.lobbyUi;
            screenHeight = movingAwardsParentRectTransform.sizeDelta.y;
            awardTrajectoryFactory = new MovingAwardTrajectoryFactory();
        }

        protected override ICollector<LobbyUiEntity> GetTrigger(IContext<LobbyUiEntity> context)
        {
            return context.CreateCollector(LobbyUiMatcher.CommandToCreateAwardImages.Added());
        }

        protected override bool Filter(LobbyUiEntity entity)
        {
            return entity.hasCommandToCreateAwardImages;
        }

        protected override void Execute(List<LobbyUiEntity> entities)
        {
            const int maxNumberOfImagesPerAwardType = 100;
            foreach (var command in entities.Select(entity => entity.commandToCreateAwardImages))
            {
                AwardTypeEnum awardTypeEnum = command.awardTypeEnum;
                DateTime spawnStartTime = command.startSpawnTime;
                int remainder = command.quantity;
                int numberOfImages;
                
                if (command.quantity > maxNumberOfImagesPerAwardType)
                {
                    numberOfImages = maxNumberOfImagesPerAwardType;
                }
                else
                {
                    numberOfImages = command.quantity;
                }


                int roundedAverageIncrement = (int) Math.Round(((decimal) command.quantity / numberOfImages),
                    MidpointRounding.AwayFromZero);
                // log.Debug($"{nameof(roundedAverageIncrement)} {roundedAverageIncrement}");
                
                int index = 0;
                while (remainder != 0)
                {
                    index++;
                    LobbyUiEntity entity = contextsLobbyUi.CreateEntity();
                    
                    Vector3 spawnPoint = MovingAwardsUiElementsStorage.Instance().GetStartPoint(awardTypeEnum);
                    Vector3 finishPoint = MovingAwardsUiElementsStorage.Instance().GetFinishPoint(awardTypeEnum);
                    
                    // log.Debug($"finish position {finishPoint.x} {finishPoint.y}");
                    List<ControlPoint> controlPoints =  awardTrajectoryFactory
                        .Create(index, spawnStartTime, spawnPoint, finishPoint, random, screenHeight);

                    // log.Debug("last control point position "+controlPoints.Last().position.x+" "+controlPoints.Last().position.y);
                    int increment;
                    if (roundedAverageIncrement < remainder)
                    {
                        increment = roundedAverageIncrement;
                    }
                    else
                    {
                        increment = remainder;
                    }
                    IconTrajectory iconTrajectory = new IconTrajectory()
                    {
                        controlPoints =  controlPoints,
                        currentControlPointIndex = 1
                    };
                    entity.AddMovingIcon( increment, iconTrajectory, awardTypeEnum);
                    entity.AddPosition(controlPoints.First().position);
                    entity.AddAlpha(0);
                    entity.AddScale(new Vector3(1,1,1));

                    remainder -= increment;
                }
            }
        }
    }
}