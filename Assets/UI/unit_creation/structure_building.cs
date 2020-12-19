using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class structure_building : MonoBehaviour
{
    public GameObject oil_prefab;
    GameObject oil_well;

    public GameObject barracks_prefab;
    GameObject barracks;

    public GameObject hq_prefab;
    GameObject hq;

    public unit_behavior ub;

    public void create_oil()
    {
        oil_well = Instantiate(oil_prefab);
        ub.target = oil_well;
        ub.changeState(unit_behavior.UnitFSM.Seek);
    }

    public void create_barracks()
    {
        barracks = Instantiate(barracks_prefab);
        ub.target = barracks;
        ub.changeState(unit_behavior.UnitFSM.Seek);
    }

    public void create_hq()
    {
        hq = Instantiate(hq_prefab);
        ub.target = hq;
        ub.changeState(unit_behavior.UnitFSM.Seek);
    }

}
