using UnityEngine;
using System.Collections;
public class Flee : AgentBehaviour
{

    //go in opposite direction from target
    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        steering.linear = transform.position - target.transform.position;
        steering.linear.Normalize();
        steering.linear = steering.linear * agent.maxAccel;
        return steering;
    }
}