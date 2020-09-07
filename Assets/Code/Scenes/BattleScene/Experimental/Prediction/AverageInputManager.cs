using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Scenes.BattleScene.Udp.Experimental;
using Plugins.submodules.SharedCode.NetworkLibrary.Udp.PlayerToServer;
using Plugins.submodules.SharedCode.Systems.InputHandling;

namespace Code.Scenes.BattleScene.Experimental.Prediction
{
    /// <summary>
    /// Достаёт из историю вводов начиная с какого-то тика. Разбивает по группам и возвращает список усреднённых вводов.
    /// Это нужно чтобы делать меньше тиков физики во премя пересимуляции.
    /// </summary>
    public class AverageInputManager
    {
        public List<AverageInputMessageModel> GetAverageInputs(List<KeyValuePair<uint, InputMessageModel>> inputs)
        {
            List<AverageInputMessageModel> result = new List<AverageInputMessageModel>();
            
            //todo достать из истории тиков сервера
            int clientToServerFrequencyCoefficient = 6;
            var groups = inputs.Split(clientToServerFrequencyCoefficient);
            InputMessageModelUtil inputMessageModelUtil = new InputMessageModelUtil();
            foreach (var group in groups)
            {
                InputMessageModel averageInputModel = inputMessageModelUtil
                    .GetAverage(group.Select(pair=>pair.Value).ToList());
                KeyValuePair<uint, InputMessageModel> firstInGroup = group.First();
                result.Add(new AverageInputMessageModel()
                {
                    inputMessageModel = averageInputModel,
                    replacedInputsIds = group.Select(item=>item.Key).ToList(),
                    inputId = group.First().Key
                });
            }
            
            return result;
        }
    }
}