using System;
using Plugins.submodules.EntitasCore.Entitas.Components;
using UnityEngine;

namespace Code.Scenes.LobbyScene.ECS.Extensions
{
    public static class MovingAwardComponentExtension
    {
        public static DateTime GetCurrentTargetArrivalTime(this MovingIconComponent movingIcon)
        {
            return movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex].arrivalTime;
        }
        
        public static Vector3 GetCurrentTargetPoint(this MovingIconComponent movingIcon)
        {
            return movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex].position;
        }

        public static bool IsRaiseUpNeeded(this MovingIconComponent movingIconComponent)
        {
            return movingIconComponent.iconTrajectory.controlPoints[movingIconComponent.iconTrajectory.currentControlPointIndex].moveToUp;
        }

        public static void TurnOffRaiseUp(this MovingIconComponent movingIconComponent)
        {
            movingIconComponent.iconTrajectory.controlPoints[movingIconComponent.iconTrajectory.currentControlPointIndex].moveToUp = false;
        }

        private static DateTime GetCurrentLineStartTime(this MovingIconComponent movingIcon)
        {
            return movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex-1].arrivalTime;
        }

        private static Vector3 GetCurrentLineStartPosition(this MovingIconComponent movingIcon)
        {
            return movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex-1].position;
        }

        private static float GetPercentageOfCoveredDistance(this MovingIconComponent movingIcon, DateTime now)
        {
            return (float) ((now - movingIcon.GetCurrentLineStartTime()).TotalMilliseconds
                            /
                            (movingIcon.GetCurrentTargetArrivalTime() - movingIcon.GetCurrentLineStartTime()).TotalMilliseconds);
        }

        private static Vector3 GetDeltaVector3(this MovingIconComponent movingIcon)
        {
            return movingIcon.GetCurrentTargetPoint() - movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex - 1].position;
        }

        public static Vector3 CalculateScale(this MovingIconComponent movingIcon,DateTime now)
        {
            Vector3 deltaScale = movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex].scale 
                                 - movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex - 1].scale;
            return movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex - 1].scale + movingIcon.GetPercentageOfCoveredDistance(now) * deltaScale;
        }

        public static float CalculateAlpha(this MovingIconComponent movingIcon, DateTime now)
        {
            float deltaAlpha = movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex].alpha - movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex - 1].alpha;
            return movingIcon.iconTrajectory.controlPoints[movingIcon.iconTrajectory.currentControlPointIndex - 1].alpha + movingIcon.GetPercentageOfCoveredDistance(now) * deltaAlpha;
        }

        public static Vector3 CalculatePosition(this MovingIconComponent movingIcon, DateTime now)
        {
            Vector3 deltaPositionForTime = movingIcon.GetDeltaVector3() * 
                                           movingIcon.GetPercentageOfCoveredDistance(now);
            return movingIcon.GetCurrentLineStartPosition() +  deltaPositionForTime;
        }
    }
}