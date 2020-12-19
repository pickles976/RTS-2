using UnityEngine;
using System.Collections;
public class Agent : MonoBehaviour
{
    public float maxSpeed = 10.0f;
    public float trueMaxSpeed;
    public float maxAccel = 30.0f;

    public float orientation;
    public float rotation;
    public Vector3 velocity;
    protected Steering steering;

    public float maxRotation = 45.0f;
    public float maxAngularAccel = 45.0f;

    void Start()
    {
        velocity = Vector3.zero;
        steering = new Steering();
        trueMaxSpeed = maxSpeed;
    }

    public void SetSteering(Steering steering,float weight)
    {
        this.steering.linear += (weight * steering.linear);
        this.steering.angular += (weight * steering.angular);
    }

    //change transform based off last frame's steering
    public virtual void Update()
    {
        Vector3 displacement = velocity * Time.deltaTime;
        displacement.y = 0;//do not allow velocity changes in the y axis

        orientation += rotation * Time.deltaTime;
        // we need to limit the orientation values
        // to be in the range (0 – 360)
        if (orientation < 0.0f)
            orientation += 360.0f;
        else if (orientation > 360.0f)
            orientation -= 360.0f;
        transform.Translate(displacement, Space.World);
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, orientation);
    }

    //update movement for the next frame
    public virtual void LateUpdate()
    {
        velocity += steering.linear * Time.deltaTime;
        rotation += steering.angular * Time.deltaTime;
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity = velocity * maxSpeed;
        }
        /*
        if (steering.angular == 0.0f)
        {
            rotation = 0.0f;
        }
        */
        if (steering.linear.sqrMagnitude == 0.0f)
        {
            velocity = Vector3.zero;
        }
        steering = new Steering();
    }

    public void speedReset()
    {
        maxSpeed = trueMaxSpeed;
    }
}
