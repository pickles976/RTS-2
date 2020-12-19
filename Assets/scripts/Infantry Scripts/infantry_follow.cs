using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infantry_follow : MonoBehaviour
{

    infantry_behavior ib;
    GameObject target;
    List<GameObject> targets = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        steeringInit();
    }

    //
    private void Update()
    {
        float d = (transform.position - target.transform.position).magnitude;

        if (d > 3.0f) //stop following if we're too close
        {
            steeringEnable();
        }
        else
        {
            steeringDisable();
        }

        //apply our rotation
        if (ib.agentScript.velocity.magnitude > 0.1f) {
            float rotation = Vector3.SignedAngle(Vector3.left, ib.agentScript.velocity, Vector3.up);
            ib.agentScript.orientation = rotation;
        }

    }

    //enable steering
    void steeringEnable()
    {
        ib.coh.enabled = true;
        ib.sep.enabled = true;
        ib.pursue.enabled = true;
    }

    //disable steering
    void steeringDisable()
    {
        ib.coh.enabled = false;
        ib.sep.enabled = false;
        ib.pursue.enabled = false;
    }

    //initialize
    public void steeringInit()
    {

        ib = gameObject.GetComponent<infantry_behavior>();
        target = ib.parent_object;
        targets = target.GetComponent<Fireteam>().fireteam;

        //add the cohesion script
        if (gameObject.GetComponent<BoidCohesion>() == null)
        {
            ib.coh = gameObject.AddComponent<BoidCohesion>();
            ib.coh.targets = targets;
            ib.coh.neighbordist = 100;
            ib.coh.weight = 5.0f;
            ib.coh.enabled = false;
        }

        //add the separation script
        if (gameObject.GetComponent<BoidSeparation>() == null)
        {
            ib.sep = gameObject.AddComponent<BoidSeparation>();
            ib.sep.targets = targets;
            ib.sep.weight = 30.0f;
            ib.sep.desiredseparation = 7.0f;
            ib.sep.enabled = false;
        }

        //add the pursue script
        if (gameObject.GetComponent<Pursue>() == null)
        {
            ib.pursue = gameObject.AddComponent<Pursue>();
            ib.pursue.enabled = false;
            ib.pursue.target = target;
        }
    }

    private void OnDestroy()
    {
        if (ib != null)
        {
            Destroy(ib.coh);
            Destroy(ib.sep);
            Destroy(ib.pursue);
        }
    }
}
