using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seek_target : MonoBehaviour
{

    public int target_id;
    public GameObject target;
    public movement_group mg;
    unit_behavior ub;

    Vector3 lastPos;

    GameObject temp_target;

    bool targetIsEnemy;

    // Start is called before the first frame update
    void Start()
    {
        ub = gameObject.GetComponent<unit_behavior>();

        lastPos = target.transform.position;

        ub.gps = gameObject.AddComponent<general_pathfinding>(); //create the general pathfinding script

        ub.mg_table.addToGroup(target, gameObject); //add ourselves to a movement group

        mg = ub.mg_table.getParentGroup(target, gameObject);

        ub.gps.target = target;
        ub.gps.mg = mg;

        temp_target = new GameObject();

        //check if the target is an enemy
        if(target.GetComponent<unit_behavior>() != null)
        {
            if(target.GetComponent<unit_behavior>().team != ub.team)
            {
                targetIsEnemy = true;
            }
        }
        else if (target.GetComponent<structure_behavior>() != null)
        {
            if (target.GetComponent<structure_behavior>().team != ub.team)
            {
                targetIsEnemy = true;
            }
        }

        //this needs to be cleaned up
        //check if we are a tank
        if (targetIsEnemy)
        {
            if (GetComponent<tank_behavior>() != null)
            {
                GetComponent<tank_behavior>().turret.GetComponent<turret_behavior>().target = target;

                if (GetComponent<tank_behavior>().turret.GetComponent<turret_attacking>() == null)
                {
                    GetComponent<tank_behavior>().turret.GetComponent<turret_behavior>().attackScript = GetComponent<tank_behavior>().turret.AddComponent<turret_attacking>();
                }
                else
                {
                    GetComponent<tank_behavior>().turret.GetComponent<turret_attacking>().enabled = true;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (targetIsEnemy == true)
        {
            seekEnemy();
        }
        else
        {
            seekPoint();
        }
        
    }
    
    void seekPoint() //seek a non-enemy target
    {

        if((target.transform.position - transform.position).magnitude > 5.0f)
        {
            ub.gps.target = target;
            ub.gps.steeringEnable();
        }
        else//we are at the target
        {
            ub.changeState(unit_behavior.UnitFSM.Idle);
            ub.idleScript.stopNeighbors = true;
            ub.idleScript.mg = mg;
            DestroyImmediate(this);
        }
    }

    void seekEnemy()
    {
        //can we shoot the target?

        if (withinRange()) //yes
        {
            ub.changeState(unit_behavior.UnitFSM.Attack);
            DestroyImmediate(this);
        }
        else //no
        {
            //can we see the target?

            if (targetVisible()) //yes
            {
                ub.gps.target = target;
                ub.gps.steeringEnable();
                lastPos = target.transform.position; //save the last position
            }
            else //no
            {
                if ((transform.position - lastPos).magnitude > 50.0f)
                {
                    //create a fake target at lastpos and move towards it
                    temp_target.transform.position = lastPos;
                    temp_target.AddComponent<Agent>();
                    ub.gps.target = temp_target;
                }
                else //target has evaded us
                {
                    ub.changeState(unit_behavior.UnitFSM.Idle);
                    DestroyImmediate(this);
                }
            }

        }
    }

    //check if we can see our target or not
    bool targetVisible()
    {
        if (target != null)
        { 
            if ((gameObject.transform.position - target.transform.position).magnitude < ub.viewDistance)
            {
                return true;
            }
        }
        return false;
    }

    //check if we can shoot at our target or not
    bool withinRange()
    {
        if (target != null)
        {
            if ((gameObject.transform.position - target.transform.position).magnitude < (ub.shootDistance * 0.85)) //move closer to the target than necessary
            {
                return true;
            }

        }
        return false;
    }

    private void OnDestroy()
    {
        Destroy(GetComponent<general_pathfinding>());
        Destroy(temp_target);
        mg.removeMember(gameObject);
    }

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up, "Seeking");
    }

}
