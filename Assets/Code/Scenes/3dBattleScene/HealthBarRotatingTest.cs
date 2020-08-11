using UnityEngine;

namespace Code.Scenes._3dBattleScene
{
    public class HealthBarRotatingTest:MonoBehaviour
    {
        private Transform cameraTransform;
        private Transform healthBarTransform;

        private void Update()
        {
            healthBarTransform.LookAt(healthBarTransform.position+cameraTransform.forward);
        }
    }
}