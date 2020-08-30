using Code.Common.Storages;
using Code.Prediction;
using Code.Scenes.BattleScene.Experimental.Prediction;
using Entitas;
using Plugins.submodules.EntitasCore.Prediction;
using Plugins.submodules.SharedCode.LagCompensation;
using Plugins.submodules.SharedCode.Logger;
using Plugins.submodules.SharedCode.Prediction;

namespace Code.Scenes.BattleScene.ECS
{
    /// <summary>
    /// Применяет новую информацию с сервера.
    /// </summary>
    public class PredictionСheckSystem:IExecuteSystem
    {
        private int lastSavedTickNumber;
        private readonly ServerGameStateBuffer serverGameStateBuffer;
        private readonly PredictionManager predictionManager;
        private readonly ILog log = LogManager.CreateLogger(typeof(PredictionСheckSystem));
        
        public PredictionСheckSystem(ServerGameStateBuffer serverGameStateBuffer, PredictionManager predictionManager)
        {
            this.serverGameStateBuffer = serverGameStateBuffer;
            this.predictionManager = predictionManager;
        }
        
        public void Execute()
        {
            int newestTickNumber = serverGameStateBuffer.GetLastSavedTickNumber();
            
            //Пришла новая информация
            if (lastSavedTickNumber < newestTickNumber)
            {
                //Обновить локальный счётчик
                lastSavedTickNumber = newestTickNumber;
                
                FullSnapshot newestFullSnapshot = serverGameStateBuffer.GetNewestGameState();
                ushort playerEntityId = PlayerIdStorage.PlayerEntityId;
                
                //проверить, что игрок правильно предсказан или пересоздать текущее состояник
                predictionManager.Reconcile(newestFullSnapshot, playerEntityId);
            }
            else
            {
                //новый тик от сервера не пришёл
            }
        }
    }
}