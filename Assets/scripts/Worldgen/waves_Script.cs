using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waves_Script : MonoBehaviour
{

    public float speed;
    public float center;
    public float delta;
    float t;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x,center,transform.position.z);
        t = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
       float new_y =  delta * Mathf.Sin( t * speed);
       transform.position = new Vector3(transform.position.x, center + new_y, transform.position.z);

    }
}
