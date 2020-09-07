using UnityEngine;

namespace Code.Scenes.BattleScene.ECS.Systems.EnvironmentSystems
{
    public static class Extensions
    {
        public static void LookAtSmooth(this Transform me, Vector3 target, float velocity)
        {
            Quaternion look = Quaternion.LookRotation(target - me.position);
            me.rotation = Quaternion.Lerp(me.rotation, look, velocity * Time.deltaTime);
        }
    }
}