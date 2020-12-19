using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//movement script for when we are in formation
public class infantry_formations : MonoBehaviour
{

    infantry_behavior ib;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = new GameObject();
        steeringInit();
    }

    // Update is called once per frame
    void Update()
    {

        float d = (transform.position - target.transform.position).magnitude;

        if (d > 1.0f) //stop following if we're too close
        {
            steeringEnable();
            //apply our rotation
            float rotation = Vector3.SignedAngle(Vector3.left, ib.agentScript.velocity, Vector3.up);
            ib.agentScript.orientation = rotation;
        }
        else
        {
            steeringDisable();
        }
    }

    //initialize steering behaviors
    void steeringInit()
    {
        ib = gameObject.GetComponent<infantry_behavior>();
        ib.seek = gameObject.AddComponent<Seek>();
        ib.seek.target = target;
        ib.seek.enabled = false;
    }

    void steeringEnable()
    {
        ib.seek.enabled = true;
    }

    void steeringDisable()
    {
        ib.seek.enabled = false;
    }

    private void OnDestroy()
    {
        Destroy(ib.seek);
        Destroy(target);
    }
}
