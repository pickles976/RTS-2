using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldSteering : AgentBehaviour
{

    ff_dictionary ff_table;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        ff_table = gameObject.GetComponent<unit_behavior>().ff_table;
    }

    public override Steering GetSteering()
    {

        Steering steering = new Steering();
        if (ff_table.contains(gameObject.GetInstanceID())) { //if there is a corresponding flowfield
            steering.linear = ff_table.getFFVector(gameObject);
        }
        else
        { //if not we generate a new one
            //gameObject.GetComponent<unit_behavior>().ffp.enabled = false;
        }

        //Debug.Log(steering.linear);
        steering.linear.Normalize();
        steering.linear = steering.linear * agent.maxAccel;
        //Debug.DrawRay(transform.position, steering.linear,Color.red);
        return steering;
    }
}
