using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resource_view : MonoBehaviour
{
    public GameObject resource_cam_prefab;
    GameObject resource_cam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (resource_cam == null)
            {
                resource_cam = Instantiate(resource_cam_prefab, transform.position,transform.rotation);
                resource_cam.transform.GetChild(0).transform.rotation = transform.GetChild(0).transform.rotation;
            }
            else
            {
                Destroy(resource_cam);
            }
        }
    }
}
