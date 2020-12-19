using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tank_behavior : MonoBehaviour
{

    public GameObject turret_prefab; //corresponding turret for this tank
    public GameObject turret;
    public Vector3 turret_offset; //offset

    public GameObject dead_tank;

    // Start is called before the first frame update
    void Start()
    {
        turret = Instantiate(turret_prefab);
        turret.GetComponent<turret_parenting>().parent = gameObject;
        turret.GetComponent<turret_parenting>().offset = turret_offset;
    }

    private void OnDestroy()
    {
        Destroy(turret);

        try //TODO: put in a destroyed tank prefab for each tank object
        {
            Instantiate(dead_tank, transform.position, transform.rotation);
        }
        catch
        {
            Debug.Log("Destroyed Tank prefab was not assigned");
        }
    }
}
