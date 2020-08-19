using System;
using Entitas;
using Plugins.submodules.SharedCode.Logger;
using UnityEngine;

namespace Code.Scenes.LobbyScene.ECS.MatchSearch.StartButton
{
    /// <summary>
    /// Вращаети растягивает картинки вокруг кнопки START в зависимости от времени.
    /// </summary>
    public class StartButtonAnimationSystem:IExecuteSystem,IInitializeSystem
    {
        private float[] baseScaleValues;
        private readonly RectTransform[] startButtonSatellites;
        private readonly DateTime systemStartTime = DateTime.Now; 
        
        private readonly ILog log = LogManager.CreateLogger(typeof(StartButtonAnimationSystem));
        
        public StartButtonAnimationSystem(LobbyUiContext contextsLobbyUi, RectTransform[] startButtonSatellites)
        {
            if (startButtonSatellites == null || startButtonSatellites.Length == 0)
            {
                throw new Exception($"{nameof(startButtonSatellites)} was empty");
            }
            this.startButtonSatellites = startButtonSatellites;
        }

        public void Execute()
        {
            for (var index = 0; index < startButtonSatellites.Length; index++)
            {
                RectTransform transform = startButtonSatellites[index];
                transform.rotation = GetRotation(index);
                transform.localScale = GetScale(index);
            }
        }

        private Vector3 GetScale(int index)
        {
            double deltaTime = (DateTime.Now - systemStartTime).TotalSeconds;
            float minimumCircleSize = 0.95f;
            float indexDependentFrequency = 2;
            float timeDependentFrequency = 2;
            float maximumAmplitude = 1 / 10f;
            float modifier = minimumCircleSize + Math.Abs((float) Math.Sin(indexDependentFrequency*index + timeDependentFrequency*deltaTime))*maximumAmplitude;
            float value = modifier * baseScaleValues[index]; 
            return new Vector3(value,value, 1);
        }

        private Quaternion GetRotation(int index)
        {
            float z = GetAngle(index);
            var rotation = Quaternion.Euler(0,0,z);
            return rotation;
        }

        private float GetAngle(int index)
        {
            float timeDependentDeceleration = 20f;
            float indexDependentAcceleration = 20f;
            return (float)(DateTime.Now - systemStartTime).TotalMilliseconds/timeDependentDeceleration  + (index+1)*indexDependentAcceleration;
        }

        public void Initialize()
        {
            baseScaleValues = new float[startButtonSatellites.Length];
            for (var index = 0; index < startButtonSatellites.Length; index++)
            {
                RectTransform startButtonSatellite = startButtonSatellites[index];
                baseScaleValues[index] = startButtonSatellite.localScale.x;
            }
        }
    }
}