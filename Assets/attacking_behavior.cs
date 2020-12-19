using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is the script attached and active during the "attacking" state
public class attacking_behavior : MonoBehaviour
{

    unit_behavior ub;

    public int target_id;
    public GameObject target;

    GameObject firingPos;

    // Start is called before the first frame update
    void Start()
    {
        ub = gameObject.GetComponent<unit_behavior>();

        firingPos = new GameObject();

        ub.seek = gameObject.AddComponent<Seek>();
        ub.seek.target = firingPos;
        ub.seek.enabled = false;

        if (GetComponent<tank_behavior>() != null)
        {
            GetComponent<tank_behavior>().turret.GetComponent<turret_behavior>().target = target;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //is target alive?

        if (isTargetAlive()) //yes
        {

            //is target within range?

            if(withinRange()){ //yes

                Vector3 ray = (target.transform.position - transform.position); //vector from us to the target
                float distance = ray.magnitude / 2;
                RaycastHit hit;

                //check for los with spherecast
                if (Physics.SphereCast(transform.position, 0.6f, ray, out hit, distance, (1<<9) | (1<<10))) //collide with units and obstacles
                {
                    ub.seek.enabled = true; //enable seek script 
                    firingPos.transform.position = getNewFiringPos(); //get new firing position
                    //Debug.DrawLine(transform.position, transform.position + ray,Color.red,0.01f);

                    //apply our rotation
                    float rotation = Vector3.SignedAngle(Vector3.left, ub.agentScript.velocity, Vector3.up);
                    ub.agentScript.orientation = rotation;
                }
                else //if los exists
                {
                    ub.seek.enabled = false; //disable seek script
                    //continue attacking

                    float rotation = Vector3.SignedAngle(Vector3.left, ray, Vector3.up);
                    ub.agentScript.orientation = rotation;
                    //Debug.DrawLine(transform.position, transform.position + ray, Color.green, 0.01f);
                }

            }
            else //not within range
            {
                Destroy(this);
            }

        }
        else //no
        {
            ub.changeState(unit_behavior.UnitFSM.Idle);
            DestroyImmediate(this);
        }

    }

    //check if the target is still alive
    bool isTargetAlive()
    {
        return target != null;
    }

    //check if we are within range
    bool withinRange()
    {
        if ((gameObject.transform.position - target.transform.position).magnitude < (ub.shootDistance))
        {
            return true;
        }

        //if target out of range, return to seek state
        ub.changeState(unit_behavior.UnitFSM.Idle);
        DestroyImmediate(this);
        return false;
    }


    //gets a new firing position by rotation around the target
    Vector3 getNewFiringPos()
    {
        Vector3 anchor = target.transform.position;
        Vector3 relPos = transform.position;

        float angle = 5.0f * (Mathf.PI / 180); // Convert to radians

        //rotate around the target
        var rotatedX = Mathf.Cos(angle) * (relPos.x - anchor.x) - Mathf.Sin(angle) * (relPos.z - anchor.z) + anchor.x;
        var rotatedZ = Mathf.Sin(angle) * (relPos.x - anchor.x) + Mathf.Cos(angle) * (relPos.z - anchor.z) + anchor.z;

        relPos.x = rotatedX;
        relPos.z = rotatedZ;

        return relPos;
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up, "Attacking");
    }

}
