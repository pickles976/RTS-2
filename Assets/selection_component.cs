using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class selection_component : MonoBehaviour
{
    //mesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/star.blend", typeof(Mesh));
    unit_behavior ub;
    GameObject shader;
    GameObject selected_ui;

    // Start is called before the first frame update
    void Start()
    {
        ub = GetComponent<unit_behavior>();

        //always show health when selected
        if (gameObject.GetComponent<draw_health>() == null)
        {
            if (gameObject.GetComponent<Fireteam>() == null)
            {
                gameObject.GetComponent<unit_behavior>().healthScript = gameObject.AddComponent<draw_health>();
                shader = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath("Assets/prefabs/SelectionObject.prefab", typeof(GameObject)));
                shader.GetComponent<Projector>().orthographicSize = gameObject.GetComponent<BoxCollider>().size.x; //scale to size of object
            }
            else
            {
                gameObject.GetComponent<Fireteam>().drawHealth();
                gameObject.GetComponent<Fireteam>().drawSelectionCircle();
            }
        }

        //check if no other copies of the UI exist
        if (ub.selected_ui.name != null)
        {
            if (GameObject.Find(ub.selected_ui.name) == null)
            {
                //instantiate UI
                selected_ui = Instantiate(ub.selected_ui);
                selected_ui.GetComponent<structure_building>().ub = ub;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1)) //if the mouse button is pressed
        {
              DestroyImmediate(ub.seekScript);
              DestroyImmediate(ub.idleScript);
              DestroyImmediate(ub.attackingScript);
              ub.target = GameObject.Find("CustomEventSystem").GetComponent<global_selection>().target;
              ub.changeState(unit_behavior.UnitFSM.Seek);
        }

        if (shader != null)
        {
            shader.transform.position = transform.position;
        }
        else if(gameObject.GetComponent<Fireteam>() != null)
        {
            gameObject.GetComponent<Fireteam>().updateSelectionCircle();
        }

    }

    void OnDestroy()
    {
        if (gameObject.GetComponent<Fireteam>() == null)
        {
            Destroy(gameObject.GetComponent<unit_behavior>().healthScript);
            Destroy(shader);
            Destroy(selected_ui);
        }
        else
        {
            gameObject.GetComponent<Fireteam>().hideHealth();
            gameObject.GetComponent<Fireteam>().hideSelectionCircle();
        }
    }

}
