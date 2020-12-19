using UnityEngine;
using System.Collections;

[System.Serializable]
public class Align : AgentBehaviour
{
    public float targetRadius = 1.0f;
    public float slowRadius = 15.0f;
    public float timeToTarget = 0.3f;

    public override Steering GetSteering()
    {

    Steering steering = new Steering();
        float targetOrientation = target.GetComponent<Agent>().orientation;
        float rotation = targetOrientation - agent.orientation;
        rotation = MapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);

        if (rotationSize < targetRadius)
        {
            return steering;
        }

        float targetRotation;

        if (rotationSize > slowRadius)
        {
            targetRotation = agent.maxRotation;
        }
        else
        {
            targetRotation = agent.maxRotation * rotationSize / slowRadius;
        }

        targetRotation *= rotation / rotationSize;
        steering.angular = targetRotation - agent.rotation;
        steering.angular /= timeToTarget;
        float angularAccel = Mathf.Abs(steering.angular);

        if (angularAccel > agent.maxAngularAccel)
        {
            steering.angular /= angularAccel;
            steering.angular *= agent.maxAngularAccel;
        }
        return steering;
    }
}