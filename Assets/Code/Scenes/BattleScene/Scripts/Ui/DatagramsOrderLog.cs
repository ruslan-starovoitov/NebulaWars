using UnityEngine;
using UnityEngine.UI;

namespace Code.Scenes.BattleScene.Scripts
{
    public class DatagramsOrderLog : MonoBehaviour
    {
        private Text text;
        private static  bool haveError;
        private static  readonly object LockObj = new object();
        
        public static void DatagramsError()
        {
            lock (LockObj)
            {
                haveError = true;
            }
        }

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        private void Update()
        {
            if (haveError)
            {
                text.text = "network ERROR";
                lock (LockObj)
                {
                    haveError = false;
                }
            }
            else
            {
                text.text = "network OK";
            }
        }
    }
}