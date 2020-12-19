using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret_parenting : MonoBehaviour
{

    public GameObject parent;
    public Vector3 offset;

    void Start()
    {
        transform.position = parent.transform.position + offset;
    }

    void LateUpdate()
    {
        transform.position = parent.transform.position + offset; //stay locked to our parent chassis
    }
}
