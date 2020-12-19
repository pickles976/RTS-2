using UnityEngine;
using System.Collections;

public class Face : Align
{
    protected GameObject targetAux;

    public override void Start()
    {
        base.Start();
        targetAux = target;
        target = new GameObject();
        target.AddComponent<Agent>();
    }

    void OnDestroy()
    {
        Destroy(target);
    }

    public override Steering GetSteering()
    {
        Vector3 direction = targetAux.transform.position - transform.position; //vector from us to target

        if (direction.magnitude > 0.0f)
        {
            float targetOrientation = Mathf.Atan2(direction.x,direction.z); //trig gets the angle between us
            targetOrientation *= Mathf.Rad2Deg;
            target.GetComponent<Agent>().orientation = targetOrientation;
        }
        return base.GetSteering();
    }


}