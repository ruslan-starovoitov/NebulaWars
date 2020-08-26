using Code.Common.Storages;
using Code.Prediction;
using Entitas;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Scenes.BattleScene.ECS
{
    public class PredictionСheckSystem:IExecuteSystem
    {
        private int lastSavedTickNumber;
        private readonly GameStateBuffer gameStateBuffer;
        private readonly PredictionManager predictionManager;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictionСheckSystem));
        
        public PredictionСheckSystem(GameStateBuffer gameStateBuffer, PredictionManager predictionManager)
        {
            this.gameStateBuffer = gameStateBuffer;
            this.predictionManager = predictionManager;
        }
        
        public void Execute()
        {
            int newestTickNumber = gameStateBuffer.GetLastSavedTickNumber();
            
            //Пришла новая информация
            if (lastSavedTickNumber < newestTickNumber)
            {
                //Обновить локальный счётчик
                lastSavedTickNumber = newestTickNumber;
                
                GameState newestGameState = gameStateBuffer.GetNewestGameState();
                GameState currentClientGameState = gameStateBuffer.GetLastShownGameState();
                ushort playerEntityId = PlayerIdStorage.PlayerEntityId;
                
                //проверить, что игрок правильно предсказан или пересоздать текущее состояник
                predictionManager.Reconcile(newestGameState, currentClientGameState.tickNumber, playerEntityId);
            }
        }
    }
}