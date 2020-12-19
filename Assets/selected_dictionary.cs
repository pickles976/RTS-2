using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selected_dictionary : MonoBehaviour
{
    //dictionary linking unique ID's to gameobjects
    public Dictionary<int, GameObject> selectedTable = new Dictionary<int, GameObject>();

    public void addSelected(GameObject go)
    {
        int id = go.GetInstanceID();

        if (!(go.GetComponent<infantry_behavior>() == null)) //if it is an infantry unit
        {
            go = go.GetComponent<infantry_behavior>().parent_object;
            id = go.GetInstanceID();
        }

        if (!(selectedTable.ContainsKey(id)))
        {

            //selecting units
            if (!(go.GetComponent<unit_behavior>() == null))
            {
                selectedTable.Add(id, go); //add to the table
                go.GetComponent<unit_behavior>().selectionScript = go.AddComponent<selection_component>(); //add a selection script
                //Debug.Log("added " + id + " to selected");
            }

            //selecting buildings
            if (!(go.GetComponent<structure_behavior>() == null))
            {
                selectedTable.Add(id, go); //add to the table

                if (go.GetComponent<structure_behavior>().selected_ui_prefab != null)
                {
                    go.GetComponent<structure_behavior>().createUI(); //instantiate the build UI
                }
            }
        }

    }

    //set a list of gameobjects to selected
    public void addMultipleSelected(List<GameObject> list)
    {
        foreach(GameObject go in list)
        {
            int id = go.GetInstanceID();
            if ( !(selectedTable.ContainsKey( id ))) //check if item is not already selected
            {
                if (!(go.GetComponent<unit_behavior>() == null))
                {
                    selectedTable.Add(id, go); //add to the table
                    go.GetComponent<unit_behavior>().selectionScript = go.AddComponent<selection_component>(); //add a selection script
                }
            }
        }
    }

    public void deselect(int id)
    {
        selectedTable.Remove(id);
    }

    //deselect specified units
    public void deselectMultiple(List<int> id_list)
    {
        foreach(int id in id_list)
        {
            Destroy(selectedTable[id].GetComponent<selection_component>()); //remove the selection script
            selectedTable.Remove(id);
        }
    }

    //deselect all units
    public void deselectAll()
    {

        foreach(KeyValuePair<int,GameObject> pair in selectedTable)
        {
            if (pair.Value != null)
            {
                Destroy(selectedTable[pair.Key].GetComponent<selection_component>()); //remove the selection script

                //removing buildings
                if (!(selectedTable[pair.Key].GetComponent<structure_behavior>() == null))
                {
                    selectedTable[pair.Key].GetComponent<structure_behavior>().destroyUI(); //instantiate the build UI
                }

            }
        }
        selectedTable.Clear();
    }

}
