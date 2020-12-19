using System.Collections;
using UnityEngine;

public class AvoidWall : Seek
{

    public float avoidDistance;
    public float lookAhead;

    // Start is called before the first frame update
    public override void Start()
    {
        avoidDistance = 7.0f;
        lookAhead = 10.0f;
        base.Start();
        target = new GameObject();
    }

    public override Steering GetSteering()
    {
        Steering steering = new Steering();
        Vector3 position = transform.position;
        Vector3 rayVector = agent.velocity.normalized * lookAhead;
        Vector3 direction = rayVector;
        RaycastHit hit;

        Debug.DrawRay(position,direction);

        //look forward
        if (Physics.Raycast(position, direction, out hit, lookAhead))
        {
            position = hit.point + (hit.normal * avoidDistance);
            target.transform.position = position;
            steering = base.GetSteering();
        }

        //look right
        direction = Quaternion.Euler(0,30,0) * rayVector;
        Debug.DrawRay(transform.position, direction * 0.7f);
        if (Physics.Raycast(transform.position, direction, out hit, lookAhead * 0.7f))
        {
            position = hit.point + ((hit.normal * avoidDistance) / (transform.position - hit.point).magnitude);
            target.transform.position += position;
            steering.linear += base.GetSteering().linear;
            steering.angular += base.GetSteering().angular;
        }

        //look right
        direction = Quaternion.Euler(0, -30, 0) * rayVector;
        Debug.DrawRay(transform.position, direction * 0.7f);
        if (Physics.Raycast(transform.position, direction, out hit, lookAhead * 0.7f))
        {
            position = hit.point + ((hit.normal * avoidDistance) / (transform.position - hit.point).magnitude);
            target.transform.position += position;
            steering.linear += base.GetSteering().linear;
            steering.angular += base.GetSteering().angular;
        }
        return steering;
    }
}
