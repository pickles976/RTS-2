using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blueprint_script : MonoBehaviour
{
    RaycastHit hit;
    public GameObject prefab;
    GameObject pumpjack;
    bool following;

    //reposition object on first frame
    private void Start()
    {
        following = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from previous mouse pointer position

        if (Physics.Raycast(ray, out hit, 50000.0f, (int)(1 << 8))) //if we hit ground
        {
            transform.position = hit.point;
        }
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from previous mouse pointer position

        if (following)
        {
            if (Physics.Raycast(ray, out hit, 50000.0f, (int)(1 << 8))) //if we hit ground
            {
                transform.position = hit.point;
            }
        }

        //onclick
        if (Input.GetMouseButtonDown(0))
        {
            //pumpjack = Instantiate(prefab,transform.position,transform.rotation);
            //Destroy(gameObject);
            following = false;

        }

    }
}
