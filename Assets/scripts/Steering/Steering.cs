using UnityEngine;
using System.Collections;

public class Steering
{
    public float angular; //rotation 0-360
    public Vector3 linear; //velocity
    public Steering()
    {
        angular = 0.0f;
        linear = new Vector3();
    }
}
