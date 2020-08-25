using Code.Common.Storages;
using Entitas;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.ECS.Systems.EnvironmentSystems
{
    public class LoadingImageSwitcherSystem : IExecuteSystem
    {
        private readonly Image loadingImage;
        private readonly ServerGameContext gameContext;

        public LoadingImageSwitcherSystem(Contexts contexts, Image loadingImage)
        {
            this.loadingImage = loadingImage;
            gameContext = contexts.serverGame;
        }
        
        public void Execute()
        {
            var playerEntity = gameContext.GetEntityWithId(PlayerIdStorage.PlayerEntityId);
            if (playerEntity == null)
            {
                TryEnableLoadingImage();
            }
            else
            {
                TryDisableLoadingImage();
            }
        }

        private void TryDisableLoadingImage()
        {
            if (loadingImage.IsActive())
            {
                loadingImage.gameObject.SetActive(false);
            }
        }

        private void TryEnableLoadingImage()
        {
            if (!loadingImage.IsActive())
            {
                loadingImage.gameObject.SetActive(true);
            }
        }
    }
}