using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class draw_health : MonoBehaviour
{

    GameObject health_bar_instance;

    void Start()
    {
        health_bar_instance = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/health_bar/health_canvas.prefab", typeof(GameObject)), transform);
    }

    private void OnDestroy()
    {
        Destroy(health_bar_instance);
    }

}
