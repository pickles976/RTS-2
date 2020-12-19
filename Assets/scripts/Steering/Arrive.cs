using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : AgentBehaviour
{

    public float targetRadius = 5.0f;
    public float slowRadius = 15.0f;
    public float timeToTarget = 0.1f;

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        Vector3 direction = target.transform.position - transform.position;
        float distance = direction.magnitude;
        float targetSpeed;

        if (distance < targetRadius) //if we are far away, do nothing
        {
            return steering;
        }

        if(distance > slowRadius)
        {
            targetSpeed = agent.maxSpeed; //proceed at maxspeed
        }
        else
        {
            targetSpeed = agent.maxSpeed * distance / slowRadius;
        }

        Vector3 desiredVelocity = direction;
        desiredVelocity.Normalize();
        desiredVelocity *= targetSpeed;
        steering.linear = desiredVelocity - agent.velocity;
        steering.linear /= timeToTarget;

        if (steering.linear.magnitude > agent.maxAccel)
        {
            steering.linear.Normalize();
            steering.linear *= agent.maxAccel;
        }
        return steering;
    }
}
