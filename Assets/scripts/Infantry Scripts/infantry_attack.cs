using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infantry_attack : MonoBehaviour
{

    public GameObject parent;
    float shootSpeed;
    GameObject projectile;
    public GameObject target;
    float range;
    float elapsedTime;
    int damage;

    infantry_behavior ib;
    unit_behavior ub;

    // Start is called before the first frame update
    void Start()
    {
        ib = GetComponent<infantry_behavior>();
        parent = ib.parent_object;
        shootSpeed = ib.shootSpeed;
        projectile = ib.projectile;
        elapsedTime = Random.Range(0, shootSpeed);
        range = GetComponent<infantry_behavior>().shootDistance;
        damage = GetComponent<infantry_behavior>().damage;

        ub = parent.GetComponent<unit_behavior>();

        if (ub.state != unit_behavior.UnitFSM.Attack)
        {
            target = parent.GetComponent<seek_target>().target;
        }
        else
        {
            target = parent.GetComponent<attacking_behavior>().target;
        }

    }

    private void OnEnable()
    {
        if (ub.state != unit_behavior.UnitFSM.Attack)
        {
            target = parent.GetComponent<seek_target>().target;
        }
        else
        {
            target = parent.GetComponent<attacking_behavior>().target;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //aim at target
        float rotation = Vector3.SignedAngle(Vector3.left, target.transform.position - transform.position, Vector3.up);
        ib.agentScript.orientation = rotation;

        elapsedTime += Time.deltaTime;
            if (elapsedTime > shootSpeed)
            {

            float distance = (transform.position - target.transform.position).magnitude;
            Vector3 variance = Vector3.zero;

            if (distance > (range / 2)) //become less accurate as distance increases
            {
                variance.x = Random.Range(-distance / range, distance / range);
                variance.y = Random.Range(-distance / range, distance / range);
                variance.z = Random.Range(-distance / range, distance / range);
                variance *= 2.0f;
            }


            Instantiate(projectile, transform.position - transform.right * 5.0f, transform.rotation * Quaternion.Euler(variance)).GetComponent<bullet_physics>().damage = damage;
                elapsedTime = 0;
            }

    }
}
