using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidCohesion : AgentBehaviour
{

    public float neighbordist = 20;
    public List<GameObject> targets;

    public override Steering GetSteering()
    {

        Steering steering = new Steering();

        int count = 0;

        foreach (GameObject other in targets) //iterate through the group of objects
        {

            if (other != null)
            {

                float d = (transform.position - other.transform.position).magnitude;
                if ((d > 0) && (d < neighbordist))
                {
                    steering.linear += other.transform.position; // Add position
                    count++;
                }

            }
        }

        if (count > 0) //average the positions of all the objects
        {
            steering.linear /= count;
            steering.linear = steering.linear - transform.position;
        }

        return steering;

    }

}
