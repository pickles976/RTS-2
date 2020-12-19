using System.Collections;
using UnityEngine;

//move towards target based on predicted velocity
//(ONLY WORKS ON OTHER AGENTS)
public class Pursue : Seek
{
    public float maxPrediction;
    public GameObject targetAux;
    public Agent targetAgent;

    public override void Start()
    {
        base.Start();
        targetAgent = base.target.GetComponent<Agent>();
        targetAux = target;
        target = new GameObject();
    }

    private void OnDestroy()
    {
        if (targetAux != null) {
            Destroy(target);
        }
    }

    public override Steering GetSteering()
    {

        Vector3 direction = targetAux.transform.position - transform.position;
        float distance = direction.magnitude;
        float speed = agent.velocity.magnitude;
        float prediction;

        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        target.transform.position = targetAux.transform.position; //update target in seek() based on auxilary target's position + aux target's velocity
        target.transform.position += targetAgent.velocity * prediction;
        return base.GetSteering();
    }

}
