using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree_script : MonoBehaviour
{
    public GameObject prefab;
    public int health = 200;

    private void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.layer == 13)//if it is a projectile
        {
            if (health < 0) {
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                Destroy(gameObject);
            }
        }
    }
}
