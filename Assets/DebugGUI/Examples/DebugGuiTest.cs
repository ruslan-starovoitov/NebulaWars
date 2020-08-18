using UnityEngine;

public class DebugGuiTest : MonoBehaviour
{
    [DebugGUIGraph(r: 0, g: 1, b: 0, autoScale: true)]
    private float bytesPerSec;

    private void Update()
    {
        bytesPerSec = Mathf.Sin(Time.time);
        DebugGUI.Graph("bytesPerSec", bytesPerSec);
    }
}