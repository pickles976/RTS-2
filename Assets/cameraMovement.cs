using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{

    float speed;
    float zoomSpeed;

    float rotateSpeed;

    float maxHeight = 300f;
    float minHeight = 10f;

    Vector2 p1;
    Vector2 p2;

    // Use this for initialization
    void Start()
    {
        rotateSpeed = 5f;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 3.6f;
            zoomSpeed = 1080.0f;
        }
        else
        {
            speed = 1.8f;
            zoomSpeed = 540.0f;
        }

        //scale speed to camera zoom
        float hsp =  Time.deltaTime * (transform.position.y) * speed * Input.GetAxis("Horizontal");
        float vsp = Time.deltaTime * (transform.position.y) * speed * Input.GetAxis("Vertical");
        float scrollSP = Time.deltaTime * (-zoomSpeed * Mathf.Log(transform.position.y) * Input.GetAxis("Mouse ScrollWheel"));


        //limit our height
        if ((transform.position.y >= maxHeight) && (scrollSP > 0))
        {
            scrollSP = 0;
        } else if ((transform.position.y <= minHeight) && (scrollSP < 0))
        {
            scrollSP = 0;
        }

        if (transform.position.y + scrollSP > maxHeight)
        {
            scrollSP = maxHeight - transform.position.y;
        }
        else if (transform.position.y + scrollSP < minHeight)
        {
            scrollSP = minHeight - transform.position.y;
        }

        Vector3 verticalMove = new Vector3(0, scrollSP,0);

        Vector3 lateralMove = hsp * transform.right; //get lateral displacement

        Vector3 forwardMove = transform.forward; //get forward displacement
        forwardMove.y = 0; //remove vertical component
        forwardMove.Normalize(); //normalize
        forwardMove *= vsp;

        Vector3 move = verticalMove + lateralMove + forwardMove;

        transform.position += move;

        getCameraRotation();

    }


    void getCameraRotation()
    {

        if (Input.GetMouseButtonDown(2)) //check if the middle mouse button was pressed
        {
            p1 = Input.mousePosition;
        }

        if(Input.GetMouseButton(2)) //check if the middle mouse button is being held down
        {
            p2 = Input.mousePosition;

            float dx = (p2 - p1).x * rotateSpeed * Time.deltaTime;
            float dy = (p2 - p1).y * rotateSpeed * Time.deltaTime;

            transform.rotation *= Quaternion.Euler(new Vector3(0,dx,0));

            transform.GetChild(0).transform.rotation *= Quaternion.Euler(new Vector3(-dy, 0, 0));

            p1 = p2;

        }

    }

}
