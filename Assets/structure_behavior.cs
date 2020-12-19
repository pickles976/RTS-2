using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class structure_behavior : MonoBehaviour
{

    //A default script attached to structures that allows them to call methods from
    //outside

    public int team;

    public building_selection_component bsc;
    public draw_health healthScript;
    public GameObject selected_ui_prefab;

    public bool producesResources;
    public int resource_amount;
    resource_generation rg;

    public int health = 100;
    public int maxHealth = 100;
    GameObject selected_ui;

    public Vector3 wayPoint;
    Vector3 spawnPoint;



    //initialize these shits so no null errors 
    void Start()
    {
        spawnPoint = transform.position + (Vector3.up * 2.0f) - (Vector3.right * 12.0f);
        wayPoint = spawnPoint;

        //attach resource generation script to structure if it generates resources
        if (producesResources)
        {
            rg = gameObject.AddComponent<resource_generation>();
            rg.setResources(resource_amount);
            rg.sb = this;
        }
    }


    public void createUI()
    {
        //check if no other copies of the UI exist
        if (GameObject.Find("unit_build_canvas") == null)
        {
            //instantiate UI
            selected_ui = Instantiate(selected_ui_prefab);
            selected_ui.GetComponent<unit_building>().sb = gameObject.GetComponent<structure_behavior>(); //give the UI a reference to this script

            //instantiate selection component
            bsc = gameObject.AddComponent<building_selection_component>(); //add a selection script
            bsc.wayPoint = wayPoint;

        }

        //pass spawn point and rally point to the UI canvas object's script
    }

    public void destroyUI()
    {
        //destroy the UI
        Destroy(selected_ui);
        //destroy the seleciton component as well
        Destroy(bsc);
    }

    public void buildObject(GameObject obj)
    {
        GameObject new_obj = Instantiate(obj, spawnPoint, transform.rotation);
        GameObject targetObj = new GameObject();
        targetObj.transform.position = wayPoint;

        unit_behavior ub = new_obj.GetComponent<unit_behavior>();
        ub.team = team;

        DestroyImmediate(ub.seekScript);
        DestroyImmediate(ub.idleScript);
        DestroyImmediate(ub.attackingScript);

        ub.target = targetObj;
        ub.changeState(unit_behavior.UnitFSM.Seek);
    }

}
