using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSeparation : Flee
{

    public float desiredseparation = 15.0f;
    public List<GameObject> targets;

    public override Steering GetSteering()
    {

        Steering steering = new Steering();
        int count = 0;

        // For every boid in the system, check if it's too close
        foreach (GameObject other in targets)
        {

            if (other != null)
            {

                float d = (transform.position - other.transform.position).magnitude;

                // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
                if ((d > 0) && (d < desiredseparation))
                {
                    // Calculate vector pointing away from neighbor
                    Vector3 diff = transform.position - other.transform.position;
                    diff.Normalize();
                    diff /= d;        // Weight by distance
                    steering.linear += diff;
                    count++;            // Keep track of how many
                }

            }
        }

        // Average -- divide by how many
        if (count > 0)
        {
            //steering.linear /= (float)count;
        }

        return steering;
    }
}
