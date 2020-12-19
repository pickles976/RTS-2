using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//gets attached to a fireteam_parent gameobject
public class Fireteam : MonoBehaviour
{

    public List<GameObject> fireteam = new List<GameObject>();
    string unit_type = "rifleman";
    public GameObject unit_prefab;
    int unit_count = 5;

    unit_behavior ub;

    public bool formationMode; //this currently only has 2 states, on and off

    VeeFormation veeFormation;

    // Start is called before the first frame update
    void Start()
    {
        ub = GetComponent<unit_behavior>();
        veeFormation = gameObject.AddComponent<VeeFormation>();

        //create x units of type, spaced out in a nice pattern
        for (int i = 0; i < unit_count; i++)
        {
            GameObject temp_obj = Instantiate(unit_prefab, transform.position + new Vector3((i % 3) * 2, 0, (i / 3) * 2), Quaternion.Euler(0, 0, 0));
            temp_obj.GetComponent<infantry_behavior>().parent_object = gameObject;
            fireteam.Add(temp_obj);
        }


    }


    private void Update()
    {
        if (ub.state == unit_behavior.UnitFSM.Attack) //check if we are attacking
        {

            int i = 0;

            foreach (GameObject member in fireteam)
            {
                member.GetComponent<infantry_attack>().enabled = true;

                member.GetComponent<infantry_formations>().enabled = true; //set the formation script

                member.GetComponent<infantry_formations>().target.transform.position = getSlot(veeFormation, i); //give the position based on location in the formation

                i++;
            }
        }
        else if (ub.state == unit_behavior.UnitFSM.Seek) //check if we are seeking a target
        {

            //if we are in regular follow mode
            if (formationMode == false)
            {

                foreach (GameObject member in fireteam)
                {
                    member.GetComponent<infantry_follow>().enabled = true; //set the follower script
                }

            }
            else //if we are in formation mode
            {

                int i = 0;

                foreach (GameObject member in fireteam)
                {
                    member.GetComponent<infantry_formations>().enabled = true; //set the formation script

                    member.GetComponent<infantry_formations>().target.transform.position = getSlot(veeFormation,i); //give the position based on location in the formation

                    i++;
                }

            }

            foreach (GameObject member in fireteam) //disable attacking
            {
                member.GetComponent<infantry_attack>().enabled = false;
            }

        }
        else //just enter formation in idle mode
        {
            int i = 0;

            foreach (GameObject member in fireteam)
            {
                member.GetComponent<infantry_formations>().enabled = true; //set the formation script

                member.GetComponent<infantry_formations>().target.transform.position = getSlot(veeFormation, i); //give the position based on location in the formation

                i++;
            }

            foreach (GameObject member in fireteam) //disable attacking
            {
                member.GetComponent<infantry_attack>().enabled = false;
            }
        }

        repositionCollider();

    } //end update


    //get the relative location of the slot for a unit in the formation
    Vector3 getSlot(FormationPattern pattern,int i)
    {

        Vector3 anchor = transform.position;
        Vector3 slotPos;
        Quaternion rotation;
        rotation = transform.rotation;

        Vector3 relPos;

        slotPos = pattern.GetSlotLocation(i);
        relPos = anchor;
        relPos += transform.TransformDirection(slotPos); //localize to the fireteam leader object


        float angle = transform.rotation.y * (Mathf.PI / 180); // Convert to radians

        //rotate around the fireteam leader object
        var rotatedX = Mathf.Cos(angle) * (relPos.x - anchor.x) - Mathf.Sin(angle) * (relPos.z - anchor.z) + anchor.x;
        var rotatedZ = Mathf.Sin(angle) * (relPos.x - anchor.x) + Mathf.Cos(angle) * (relPos.z - anchor.z) + anchor.z;

        relPos.x = rotatedX;
        relPos.z = rotatedZ;

        return relPos; 
    }

    void repositionCollider()
    {
        Vector3 newPos = Vector3.zero;

        foreach(GameObject member in fireteam)//get the average position of all of the units
        {
            newPos += member.transform.position;
        }

        newPos /= fireteam.Count; //average the positions
        //newPos.y = GetComponent<BoxCollider>().center.y;

        GetComponent<BoxCollider>().center = transform.InverseTransformPoint(newPos);
    }

    public void drawHealth()
    {
        foreach(GameObject unit in fireteam)
        {
            unit.GetComponent<infantry_behavior>().healthScript = unit.AddComponent<draw_health>();
        }
    }

    public void drawSelectionCircle()
    {
        foreach (GameObject unit in fireteam)
        {
            unit.GetComponent<infantry_behavior>().selectionObject = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/prefabs/SelectionObject.prefab", typeof(GameObject)));
            unit.GetComponent<infantry_behavior>().selectionObject.GetComponent<Projector>().orthographicSize = 4 * unit.GetComponent<BoxCollider>().size.x; //scale to size of object
        }
    }

    public void hideHealth()
    {
        foreach (GameObject unit in fireteam)
        {
            Destroy(unit.GetComponent<infantry_behavior>().healthScript, 0.5f);
        }
    }

    public void hideSelectionCircle()
    {
        foreach (GameObject unit in fireteam)
        {
            Destroy(unit.GetComponent<infantry_behavior>().selectionObject);
        }
    }

    public void updateSelectionCircle()
    {
        foreach (GameObject unit in fireteam)
        {
            unit.GetComponent<infantry_behavior>().selectionObject.transform.position = unit.transform.position;
        }
    }

}
