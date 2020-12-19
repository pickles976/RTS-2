using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class turret_attacking : MonoBehaviour
{

    //this is attached in seek_target
    public GameObject target;
    float range;
    public GameObject projectile;
    public GameObject secondaryProjectile;

    public bool isTargetInfantry;

    float elapsedTime;
    float shootRate;
    int damage;
    int secondaryDamage;

    // Start is called before the first frame update
    void Start()
    {
        projectile = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/prefabs/projectile_tank.prefab", typeof(GameObject));
        secondaryProjectile = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/prefabs/projectile_rifle.prefab", typeof(GameObject));

        target = GetComponent<turret_behavior>().target;
        range = GetComponent<turret_behavior>().parent.GetComponent<unit_behavior>().shootDistance;
        damage = GetComponent<turret_behavior>().damage;
        secondaryDamage = GetComponent<turret_behavior>().secondaryDamage;
        shootRate = GetComponent<turret_behavior>().shootRate;
        elapsedTime = Random.Range(0, shootRate);

        isTargetInfantry = false;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (inRange())
        {
            if (elapsedTime > shootRate) {

                float distance = (transform.position - target.transform.position).magnitude;
                Vector3 variance = Vector3.zero;

                if (!isTargetInfantry) //check if we have an infantry target or an armor target
                {
                    if (distance > (range / 2)) //become less accurate as distance increases
                    {
                        variance.x = Random.Range(-distance / range, distance / range);
                        variance.y = Random.Range(-distance / range, distance / range);
                        variance.z = Random.Range(-distance / range, distance / range);
                        variance *= 1.0f;
                    }

                    elapsedTime = 0;
                    shootCannon(variance);
                }
                else
                {
                        //variance.x = Random.Range(-distance / range, distance / range);
                        variance.y = Random.Range(-distance / range, distance / range);
                        variance.z = Random.Range(-distance / range, distance / range);
                        variance *= 2.0f;

                    shootMG(variance);
                    elapsedTime -= Time.deltaTime * 5.0f;
                }

            }
        }
    }

    bool inRange() //check if target is in range
    {
        if (target != null)
        {
            return ((target.transform.position - transform.position).magnitude < range);
        }
        return false;
    }

    void shootMG(Vector3 variance)
    {
        float rotation = Vector3.SignedAngle(Vector3.forward, (target.transform.position) - transform.position, Vector3.right);
        Instantiate(secondaryProjectile, transform.position - (transform.right * 7.0f) + transform.up * 0.6f, transform.rotation * Quaternion.Euler(new Vector3(-rotation, 0, 0)) * Quaternion.Euler(variance)).GetComponent<bullet_physics>().setValues(secondaryDamage,gameObject.GetComponent<turret_behavior>().parent.GetComponent<unit_behavior>().team);
    }

    void shootCannon(Vector3 variance)
    {
        float rotation = Vector3.SignedAngle(Vector3.forward, (target.transform.position) - transform.position, Vector3.right);
        Instantiate(projectile, transform.position - (transform.right * 7.0f) + transform.up * 0.6f, transform.rotation * Quaternion.Euler(new Vector3(-rotation, 0, 0)) * Quaternion.Euler(variance)).GetComponent<bullet_physics>().setValues(damage, gameObject.GetComponent<turret_behavior>().parent.GetComponent<unit_behavior>().team);
    }
}
