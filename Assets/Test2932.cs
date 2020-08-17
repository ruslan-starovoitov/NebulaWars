using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2932 : MonoBehaviour
{
    public GameObject defaultWarship;
    public GameObject bakedLightWarship;
    
    private void Start()
    {
        Instantiate(defaultWarship, new Vector3(-15, 0, 0), Quaternion.identity);
        Instantiate(bakedLightWarship, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
