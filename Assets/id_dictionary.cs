using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class id_dictionary : MonoBehaviour
{
    //get ID's with go.GetInstanceID();
    //links our relevant GameObjects to a table for quick access

    //dictionary linking unique ID's to gameobjects
    public Dictionary<int, GameObject> idTable = new Dictionary<int, GameObject>();

    //add object to dictionary
    public void addObject(GameObject go)
    {

        int id = go.GetInstanceID();

        //check that our key does not already exist
        if (!(idTable.ContainsKey(id)))
        {
            //add object to table
            idTable.Add(id, go);

        }
    }

    //get object from ID
    public GameObject getObject(int id)
    {

        return idTable[id];

    }

    //check if the ID is in the table
    public bool containsID(int id)
    {

        return (idTable.ContainsKey(id));

    }

    //removes an entry from the ID table
    public void removeEntry(int id)
    {

        idTable.Remove(id);

    }
}
