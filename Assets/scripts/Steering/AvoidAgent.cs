using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidAgent : AgentBehaviour
{
    public float collisionRadius = 5.0f;
    public List<GameObject> targets;

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        float shortestTime = Mathf.Infinity;
        GameObject firstTarget = null;
        float firstMinSeparation = 0.0f;
        float firstDistance = 0.0f;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;

        //Debug.Log("Targets in AvoidAgent: " + targets.Count);

        foreach (GameObject t in targets)
        {

            Debug.Log(t);
            Vector3 relativePos;
            Agent targetAgent = t.GetComponent<Agent>();
            relativePos = t.transform.position - transform.position;
            //Debug.Log("Agent is: " + agent);
            //Debug.Log("Other agent is: " + targetAgent);
            Vector3 relativeVel = targetAgent.velocity - agent.velocity;
            float relativeSpeed = relativeVel.magnitude;
            float timeToCollision = Vector3.Dot(relativePos, relativeVel);
            timeToCollision /= relativeSpeed * relativeSpeed * -1;
            float distance = relativePos.magnitude;
            float minSeparation = distance - relativeSpeed * timeToCollision;

            if (minSeparation > 2 * collisionRadius)
            {
                continue;
            }
            if (timeToCollision > 0.0f && timeToCollision < shortestTime)
            {
                shortestTime = timeToCollision;
                firstTarget = t;
                firstMinSeparation = minSeparation;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        }

        if (firstTarget == null)
        {
            return steering;
        }
        if (firstMinSeparation <= 0.0f || firstDistance < 2 * collisionRadius)
        {
            firstRelativePos = firstTarget.transform.position;
        }
        else
        {
            firstRelativePos += firstRelativeVel * shortestTime;
        }

        firstRelativePos.Normalize();
        steering.linear = -firstRelativePos * agent.maxAccel;
        return steering;

    }
}
