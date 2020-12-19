using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret_behavior : MonoBehaviour
{
    public GameObject target; //set in seek_target
    public GameObject parent; //parent object
    public float shootRate;
    public int damage;
    public int secondaryDamage;

    public Agent agent;

    public turret_attacking attackScript;

    void Start()
    {
        parent = GetComponent<turret_parenting>().parent;
    }

    void LateUpdate()
    {
        if (target != null) //turn turret towards target
        {

            GetComponent<turret_attacking>().target = target;

            if (target.GetComponent<Agent>() != null) {
                agent = target.GetComponent<Agent>();

                float rotation = Vector3.SignedAngle(Vector3.left, (target.transform.position + (agent.velocity / 3.0f)) - transform.position, Vector3.up);
                transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
            }
            else
            {
                float rotation = Vector3.SignedAngle(Vector3.left, (target.transform.position) - transform.position, Vector3.up);
                transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
            }
        }
        else //face turret forward
        {

            if (!checkForEnemy()) //check for enemies while moving
            {
                transform.rotation = parent.transform.rotation;
                if (GetComponent<turret_attacking>() != null)
                {
                    GetComponent<turret_attacking>().enabled = false;
                }
            }

        }
    }

    //check for an enemy
    bool checkForEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, parent.GetComponent<unit_behavior>().viewDistance, (int)(1 << 9)); //check for units within view

        if (colliders.Length > 0)   //if we find any
        {

            float minDistance = float.MaxValue;
            int closest = -1;

            for (int i = 0; i < colliders.Length; i++) //loop thru all colliders
            { 
                if (colliders[i].transform.gameObject.GetComponent<unit_behavior>().team != parent.GetComponent<unit_behavior>().team) //check if they are on our team
                { 
                    if ((transform.position - colliders[i].transform.position).magnitude < minDistance) //check which enemy is the closest
                    {
                        closest = i;
                        minDistance = (transform.position - colliders[i].transform.position).magnitude; //save it
                    }
                }
            }

            if (closest != -1) //if there is an enemy
            {
                target = colliders[closest].transform.gameObject;    //get target from the closest enemy
                Debug.Log(GetComponent<turret_attacking>() == null);
                if (GetComponent<turret_attacking>() != null)
                {
                    GetComponent<turret_attacking>().enabled = true;
                }
                else
                {
                    attackScript = gameObject.AddComponent<turret_attacking>();
                }

                //tell our turret whether or not it is attacking an infantry target
                if (target.GetComponent<Fireteam>() == null)
                {
                    attackScript.isTargetInfantry = false;
                }
                else
                {
                    attackScript.isTargetInfantry = true;
                }

                return true;

            }//endif closest

        }//end if colliders > 0

        return false;
    }

}
