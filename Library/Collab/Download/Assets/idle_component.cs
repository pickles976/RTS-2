using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is the script attached and active during the "idle" state
public class idle_component : MonoBehaviour
{

    unit_behavior ub;

    //this is how we stop our entire group at the same time, rather than having everyone try to fight over the same spot
    public movement_group mg;
    public bool stopNeighbors = false;

    void Start()
    {
        ub = gameObject.GetComponent<unit_behavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopNeighbors) //stop neighbors
        {
            stopNeighbors = neighborStopper();
        }


        if (checkForEnemy()) //if we find an enemy, exit idle mode
        {
            DestroyImmediate(this);
        }
    }

    //check for an enemy
    bool checkForEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ub.viewDistance, (int)(1<<9 | 1<<14)); //check for units within view

        if (colliders.Length > 0)   //if we find any
        {
            float minDistance = float.MaxValue;
            int closest = -1;

            for (int i = 0; i < colliders.Length;i++) { //loop thru all colliders

                if (colliders[i].transform.gameObject.GetComponent<unit_behavior>() != null)
                {
                    if (colliders[i].transform.gameObject.GetComponent<unit_behavior>().team != ub.team)
                    { //check if they are on our team

                        if ((transform.position - colliders[i].transform.position).magnitude < minDistance) //check which enemy is the closest
                        {

                            closest = i;   //get target
                            minDistance = (transform.position - colliders[i].transform.position).magnitude; //save it

                        }
                    }
                }
                else if (colliders[i].transform.gameObject.GetComponent<structure_behavior>() != null)
                {
                    if (colliders[i].transform.gameObject.GetComponent<structure_behavior>().team != ub.team)
                    { //check if they are on our team

                        if ((transform.position - colliders[i].transform.position).magnitude < minDistance) //check which enemy is the closest
                        {

                            closest = i;   //get target
                            minDistance = (transform.position - colliders[i].transform.position).magnitude; //save it

                        }
                    }
                }
            }

            if (closest != -1) //if there is an enemy
            {

                ub.target = colliders[closest].transform.gameObject;    //get target
                ub.changeState(unit_behavior.UnitFSM.Seek);
                return true;

            }//endif closest

        }
        return false;
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up, "Idle");
        // Draw a yellow sphere at the transform's position
        if (stopNeighbors)
        {
            Gizmos.color = new Color(1,1,1,0.25f);
            Gizmos.DrawSphere(transform.position, 10.0f);
        }
    }

    //wait for neighbors to arrive so we can tell them to stop
    bool neighborStopper()
    {

        bool shouldContinue = true;

        //TODO:
        //if the movement group no longer exists in the table, we should return false

        Collider[] colliders = Physics.OverlapSphere(transform.position, 10.0f, (int)(1 << 9)); //check for units within our sphere

        if (colliders.Length > 0)   //if we find any
        {
            float minDistance = float.MaxValue;
            int closest = -1;

            for (int i = 0; i < colliders.Length; i++)
            { //loop thru all colliders
                GameObject go = colliders[i].transform.gameObject;
                unit_behavior unitb = go.GetComponent<unit_behavior>();
                if (unitb.team == ub.team) //check if they are on our team
                {
                    if(unitb.state == unit_behavior.UnitFSM.Seek) //check if they are in a seeking state
                    {
                        if (unitb.seekScript.mg == mg) //check if they were in our movement group
                        {
                            DestroyImmediate(unitb.seekScript);
                            unitb.changeState(unit_behavior.UnitFSM.Idle);
                            unitb.idleScript.stopNeighbors = true;
                            unitb.idleScript.mg = mg;
                            shouldContinue = false;
                        }
                    }                     
                }
            }

        }

        return shouldContinue; //keep trying to stop neighbors
    }

    private void OnDestroy()
    {
        Destroy(this);
    }

}
