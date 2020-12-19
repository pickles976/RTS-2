using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_physics : MonoBehaviour
{

    public float speed;
    public int damage;
    int team;

    private void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= transform.right * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject,0.01f);

        if (collision.gameObject.GetComponent<unit_behavior>() != null) //check if we hit something with unit behavior
        {
            if (collision.gameObject.GetComponent<unit_behavior>().team != team) //check team
            {

                collision.gameObject.GetComponent<unit_behavior>().health -= damage;

                if (collision.gameObject.GetComponent<unit_behavior>().health <= 0)
                {
                    Destroy(collision.gameObject);
                }

            }
        }
        else if (collision.gameObject.GetComponent<infantry_behavior>() != null) //check if we hit infantry
        {
            if (collision.gameObject.GetComponent<infantry_behavior>().parent_object.GetComponent<unit_behavior>().team != team) //check team
            {

                collision.gameObject.GetComponent<infantry_behavior>().health -= damage;

                if (collision.gameObject.GetComponent<infantry_behavior>().health <= 0)
                {
                    Destroy(collision.gameObject);
                }

            }
        }
        else if (collision.gameObject.GetComponent<structure_behavior>() != null) //check if we hit something with unit behavior
        {
            if (collision.gameObject.GetComponent<structure_behavior>().team != team) //check team
            {

                collision.gameObject.GetComponent<structure_behavior>().health -= damage;

                if (collision.gameObject.GetComponent<structure_behavior>().health <= 0)
                {
                    Destroy(collision.gameObject);
                }

            }
        }
        else if (collision.gameObject.layer == 10)
        {
            if (collision.gameObject.GetComponent<tree_script>() != null)
            {
                collision.gameObject.GetComponent<tree_script>().health -= damage;
            }

        }

    }

    public void setValues(int _damage,int _team)
    {
        damage = _damage;
        team = _team;
    }
}
