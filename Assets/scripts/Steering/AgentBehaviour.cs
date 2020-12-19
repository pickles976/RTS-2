using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AgentBehaviour : MonoBehaviour
{
    public float weight = 1.0f;

    public GameObject target; //pursuit vars
    protected Agent agent; //parent agent
    public Vector3 dest;

    public float maxSpeed = 50.0f; //vars for more complex behaviors
    public float maxAccel = 50.0f;
    public float maxRotation = 5.0f;
    public float maxAngularAccel = 5.0f;


    public virtual void Start()
    {
        agent = gameObject.GetComponent<Agent>();
    }

    public virtual void Update()
    {
        agent.SetSteering(GetSteering(),weight);
    }

    public float MapToRange(float rotation) //limit a given rotation to between -180 and 180
    {
        rotation %= 360.0f;
        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
    }

    //ALL MOVEMENT LOGIC GOES IN HERE
    public virtual Steering GetSteering()
    {
        return new Steering();
    }

}
