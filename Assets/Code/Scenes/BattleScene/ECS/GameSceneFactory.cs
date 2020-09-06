using UnityEngine.SceneManagement;

namespace Code.Scenes.BattleScene.ECS
{
    public class GameSceneFactory
    {
        public Scene Create()
        {
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
            Scene matchScene = SceneManager.LoadScene("EmptyScene", loadSceneParameters);
            return matchScene;
        }
    }
}