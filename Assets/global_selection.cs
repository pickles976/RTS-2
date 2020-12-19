using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class global_selection : MonoBehaviour
{

    public GameObject target;

    id_dictionary id_table;
    selected_dictionary selected_table;
    RaycastHit hit;

    bool dragSelect; //marquee selection flag

    //COLLIDER VARIABLES
    //====================================================================================================

    MeshCollider selectionBox;    //use this to detect selected objects
    Mesh selectionMesh;

    //the bounds of our marquee object
    Vector3 p1;
    Vector3 p2;

    //corners in 2D
    Vector2[] corners;

    //vertices of our box
    Vector3[] verts;


    void Start()
    {
        id_table = gameObject.GetComponent<id_dictionary>();
        selected_table = gameObject.GetComponent<selected_dictionary>();
        dragSelect = false;
    }

    // Update is called once per frame
    void Update()
    {

        //0. when right mouse button clicked
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from previous mouse pointer position

            if (Physics.Raycast(ray, out hit, 50000.0f, (int)(1 << 9))) //if we hit a unit
            {
                //Debug.Log("clicked on a unit");
                target = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, 50000.0f, (int)(1 << 8))) //if we hit ground
            {
                target = new GameObject();
                target.transform.position = hit.point;
            }
            else if (Physics.Raycast(ray, out hit, 50000.0f, (int)(1 << 14)))
            {
                target = hit.transform.gameObject;
            }
        }


        // 1. when left mouse button clicked
        if (Input.GetMouseButtonDown(0))
        {
            p1 = Input.mousePosition;
        }

        // 2. while mouse button held
        if (Input.GetMouseButton(0))
        {
            if ((p1 - Input.mousePosition).magnitude > 40) //if the mouse has moved a lot, then we enter marquee mode
            {
                dragSelect = true;
            }
        }

        // 3. when mouse button up
        if (Input.GetMouseButtonUp(0))
        {

            if (dragSelect == false) //differentiating between a rapid click cycle vs a drag-select
            {
                Ray ray = Camera.main.ScreenPointToRay(p1); //raycast from previous mouse pointer position

                if (Physics.Raycast(ray, out hit, 50000.0f,(int)(~(1<<8)))) ///if we hit something that isn't ground
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //Debug.Log("Inclusive Select");
                        selected_table.addSelected(hit.transform.gameObject);
                    }
                    else
                    {
                        //Debug.Log("Exclusive Select");
                        selected_table.deselectAll();
                        selected_table.addSelected(hit.transform.gameObject);

                    }
                }
                else //if we didn't hit something
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //do nothing
                    }
                    else
                    {
                        selected_table.deselectAll();
                    }
                }

            }
            else //marquee select
            {

                verts = new Vector3[4]; // initialize vertices
                int i = 0;
                p2 = Input.mousePosition; //get last mouse position
                corners = getBoundingBox(p1, p2); //get 4 corners of our box

                foreach(Vector2 corner in corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(corner); //cast out to world space
                    if (Physics.Raycast(ray, out hit, 50000.0f,(1<<8))) ///if we hit something
                    {
                        verts[i] = new Vector3(hit.point.x,0,hit.point.z);

                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), hit.point, Color.red, 1.0f);

                    }
                    i++;
                }

                //generate box from 4 vertices
                selectionMesh = generateSelectionMesh(verts);

                selectionBox = gameObject.AddComponent<MeshCollider>();
                selectionBox.sharedMesh = selectionMesh;
                selectionBox.convex = true;
                selectionBox.isTrigger = true;

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    selected_table.deselectAll();
                }

                //destroy our slection box after 1/50th of a second
                Destroy(selectionBox, 0.02f);

            }//end marquee select

            dragSelect = false;

        }//end mousebutton up


    } //end update

    //check collisions with our dynamically created box collider
    //===========================================================================================================================//
    private void OnTriggerEnter(Collider other)
    {

        selected_table.addSelected(other.gameObject);

    }


    //draw marquee box
//===========================================================================================================================//

    void OnGUI()
    {
            if (dragSelect == true)
            {
                // Create a rect from both mouse positions
                var rect = Utils.GetScreenRect(p1, Input.mousePosition);
                Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
    }


    //create a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] getBoundingBox(Vector2 p1, Vector2 p2)
    {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (p1.x < p2.x) //if p1 is to the left of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else //if p1 is below p2
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else //if p1 is to the right of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else //if p1 is below p2
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }

        }

            Vector2[] corners = { newP1,newP2,newP3,newP4 }; //the corners of the bounding box in an array
            return corners;

    }


    //generate a mesh from the 4 bottom points
    Mesh generateSelectionMesh(Vector3[] corners)
    {
        Vector3[] verts = new Vector3[8]; //get the verts
        int[] tris = {0,1,2, 2,1,3, 4,6,0, 0,6,2, 6,7,2, 2,7,3, 7,5,3, 3,5,1, 5,0,1, 1,4,0, 4,5,6, 6,5,7 }; //map the tris of our cube

        for (int i = 0; i < 4; i++) //pass in the bottom vertices
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++) // pass in the top vertices
        {
            verts[j] = (corners[j - 4] + Vector3.up * 100.0f);
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;

    }

}
