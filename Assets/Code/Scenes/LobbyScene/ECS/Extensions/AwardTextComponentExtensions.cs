using System;
using Plugins.submodules.EntitasCore.Entitas.Components;
using UnityEngine;

namespace Code.Scenes.LobbyScene.ECS.Extensions
{
    public static class AwardTextComponentExtensions
    {
        public static Vector3 CalculatePosition(this AwardTextComponent awardTextComponent, DateTime now)
        {
            return awardTextComponent.startPosition + new Vector3(0,80)
                *GetDistanceCoveragePercentage(awardTextComponent, now);
        }

        public static float GetDistanceCoveragePercentage(this AwardTextComponent awardTextComponent, DateTime now)
        {
            float distanceCoveragePercentage = (float) ((now - awardTextComponent.creationTime).TotalMilliseconds
                                                        /
                                                        (awardTextComponent.fadeTime - awardTextComponent.creationTime).TotalMilliseconds)
                ;
            return distanceCoveragePercentage;
        }
    }
}