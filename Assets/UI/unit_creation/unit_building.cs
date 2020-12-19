using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit_building : MonoBehaviour
{
    public GameObject tank_prefab;

    public GameObject riflemen_prefab;

    public structure_behavior sb;

    public void create_tank()
    {
        sb.buildObject(tank_prefab);
    }

    public void create_riflemen()
    {
        sb.buildObject(riflemen_prefab);
    }


}
