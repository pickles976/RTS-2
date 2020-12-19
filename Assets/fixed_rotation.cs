using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fixed_rotation : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 45, 0);
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 45, 0) ;
    }

}
