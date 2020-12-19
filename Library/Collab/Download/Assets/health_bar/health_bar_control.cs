using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class health_bar_control : MonoBehaviour
{

    //get the health from the parent
    public float health;
    public float maxHealth;
    public unit_behavior ub;
    public infantry_behavior ib;
    Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        //grab the health script from the parent
        if (this.transform.parent.transform.parent.transform.parent.GetComponent<unit_behavior>() != null)
        {
            ub = this.transform.parent.transform.parent.transform.parent.GetComponent<unit_behavior>();
        }
        else
        {
            ib = this.transform.parent.transform.parent.transform.parent.GetComponent<infantry_behavior>();
        }

        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(ub != null)
        {
            health = ub.health;
            maxHealth = ub.maxHealth;
        }
        else 
        {
            health = ib.health;
            maxHealth = ib.maxHealth;
        }

        //access the image (itself)
        gameObject.GetComponent<Image>().fillAmount = health / maxHealth;

        transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.up);
    }
}
