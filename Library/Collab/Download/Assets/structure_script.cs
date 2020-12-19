using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class structure_script : MonoBehaviour
{

    //A default script attached to structures that allows them to call methods from
    //outside

    public GameObject selected_ui_prefab;
    GameObject selected_ui;
    

    public void createUI()
    {
        //check if no other copies of the UI exist
        if (GameObject.Find("unit_build_canvas") == null)
        {
            //instantiate UI
            selected_ui = Instantiate(selected_ui_prefab);
            selected_ui.GetComponent<unit_building>().spawnPoint = gameObject.transform.position + (Vector3.up * 2.0f) - (Vector3.right * 12.0f);
            selected_ui.GetComponent<unit_building>().rotation = gameObject.transform.rotation;
        }

        //pass spawn point and rally point to the UI canvas object's script
    }

    public void destroyUI()
    {
        //destroy the UI
        Destroy(selected_ui);
    }

}
