using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class building_selection_component : MonoBehaviour
{
    //mesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/star.blend", typeof(Mesh));
    structure_behavior sb;
    GameObject shader;

    GameObject waypointObject;

    public Vector3 wayPoint;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        sb = GetComponent<structure_behavior>();

        //always show health when selected
        if (gameObject.GetComponent<draw_health>() == null)
        {
                sb.healthScript = gameObject.AddComponent<draw_health>();
                shader = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/prefabs/SelectionObject.prefab", typeof(GameObject)));
                shader.GetComponent<Projector>().orthographicSize = gameObject.GetComponent<BoxCollider>().size.x * 1.5f; //scale to size of object
        }

        waypointObject = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/prefabs/Buildings/waypoint.prefab", typeof(GameObject)));
        waypointObject.transform.position = wayPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (shader != null)
        {
            shader.transform.position = transform.position;
        }

        //update the waypoint
        if (Input.GetMouseButtonDown(1)) //if the mouse button is pressed
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from previous mouse pointer position

            if (Physics.Raycast(ray, out hit, 50000.0f)) //if we hit the ground
            {
                //Debug.Log("clicked on a unit");
                wayPoint = hit.point;
                sb.wayPoint = wayPoint;
                waypointObject.transform.position = wayPoint;
            }
        }
    }

    void OnDestroy()
    {
            Destroy(sb.healthScript);
            Destroy(shader);
            Destroy(waypointObject);
    }

}
