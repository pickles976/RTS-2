using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class general_pathfinding : MonoBehaviour
{
    public GameObject target;
    public movement_group mg;
    unit_behavior ub;

    Vector3 lastPos;

    bool seekMode;

    private void Update()
    {
        //apply our rotation
        if (ub.agentScript.velocity.magnitude > 0.1f) {
            float rotation = Vector3.SignedAngle(Vector3.left, ub.agentScript.velocity, Vector3.up);
            ub.agentScript.orientation = rotation;
        }


        ub.agentScript.maxSpeed = mg.maxSpeed;
        steeringEnable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Object " + gameObject.GetInstanceID() + " is now seeking an enemy");
        ub = gameObject.GetComponent<unit_behavior>();

        lastPos = target.transform.position;

        ub.mg_table.addToGroup(target, gameObject); //add ourselves to a movement group
        mg = ub.mg_table.getParentGroup(target, gameObject);

        //Debug.Log("Object " + gameObject.GetInstanceID() + " is a Leader: " + ub.isMgLeader);

        steeringInit();
    }

    public void steeringInit()
    {
        //pursue the target
        if (target.GetComponent<Agent>() != null) {
            ub.pursue = gameObject.AddComponent<Pursue>();
            ub.pursue.target = target;
            ub.pursue.enabled = true;
            seekMode = false;
        }
        else
        {
            ub.seek = gameObject.AddComponent<Seek>();
            ub.seek.target = target;
            ub.seek.enabled = false;
            seekMode = true;
        }

        //this script generates the flowfield
        ub.ffp = gameObject.AddComponent<flowfield_pathfinding>();
        ub.ffp.setDestination(target.transform.position);
        ub.ffp.enabled = false;

        //this script follows the flowfield
        ub.ffs = gameObject.AddComponent<FlowFieldSteering>();
        ub.ffs.weight = 0.5f;
        ub.ffs.enabled = false;

        //avoids other agents
        ub.boidSeparation = gameObject.AddComponent<BoidSeparation>();
        ub.boidSeparation.targets = mg.group;
        ub.boidSeparation.weight = 50.0f;
        ub.boidSeparation.enabled = false;

        //groups units together
        ub.boidCohesion = gameObject.AddComponent<BoidCohesion>();
        ub.boidCohesion.targets = mg.group;
        ub.boidCohesion.weight = 0.4f;
        ub.boidCohesion.enabled = false;

        //avoid walls
        /*
        ub.avoidWall = gameObject.AddComponent<AvoidWall>();
        ub.avoidWall.target = target;
        ub.avoidWall.weight = 5.0f;
        ub.avoidWall.enabled = false;
        */
    }


    public void steeringEnable()
    {

            Vector3 ray = ((target.transform.position + Vector3.up) - transform.position); //vector from us to the target
            float distance = ray.magnitude;
            RaycastHit hit;

        //check for line-of-sight
        if (Physics.SphereCast(transform.position, 2.0f, ray, out hit, distance, (1 << 10)) || Physics.Raycast(transform.position,ray,out hit,distance,(1<<8)))
        {
                ub.ffp.enabled = true;
                ub.ffs.enabled = true;
                //Debug.DrawRay(transform.position, ray, Color.red);

                if (seekMode)
                {
                ub.seek.enabled = false;
                }
                else
                {
                ub.pursue.enabled = false;
                }

        }
        else
        {
                if (seekMode)
                {
                    ub.seek.enabled = true;
                }
                else
                {
                    ub.pursue.enabled = true;
                }

                ub.ffp.enabled = false;
                ub.ffs.enabled = false;

            //Debug.DrawRay(transform.position, ray, Color.yellow);

        }//end raycasting

            ub.boidSeparation.enabled = true;
            ub.boidCohesion.enabled = true;
            //ub.avoidWall.enabled = true;

    }

    //turn off all steering
    public void steeringDisable()
    {
        Destroy(ub.pursue);
        //Destroy(ub.avoidWall);
        Destroy(ub.ffp);
        Destroy(ub.ffs);
        Destroy(ub.boidSeparation);
        Destroy(ub.boidCohesion);
        Destroy(ub.seek);

        ub.mg_table.removeFromGroup(target, gameObject);

        //Destroy(this);
    }

    //if we collide with an obstacle then it means the world has been updated. Calculate a new path accordingly
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == 10) //check if we have touched an obstacle collider
        {
            ub.boidSeparation.enabled = false;
            //transform.position += (transform.position - collision.contacts[0].point);

            Vector3 pushBack = (transform.position - collision.contacts[0].point);
            pushBack.y = 0;
            ub.agentScript.velocity += pushBack;
        }
    }

    private void OnDestroy()
    {
        Destroy(GetComponent<Pursue>());
        Destroy(GetComponent<flowfield_pathfinding>());
        Destroy(GetComponent<FlowFieldSteering>());
        Destroy(GetComponent<BoidSeparation>());
        Destroy(GetComponent<BoidCohesion>());
        Destroy(GetComponent<Seek>());
        //Destroy(GetComponent<AvoidWall>());
    }

}
