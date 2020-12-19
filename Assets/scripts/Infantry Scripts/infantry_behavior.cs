using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infantry_behavior : MonoBehaviour
{
    public Agent agentScript;

    public draw_health healthScript; //script that draws a healthbar
    public float maxHealth = 10.0f;
    public float health = 10.0f;

    public GameObject selectionObject;

    public float shootDistance;

    public GameObject parent_object;

    public BoidCohesion coh;
    public BoidSeparation sep;
    public Pursue pursue;
    public Seek seek;

    public infantry_follow followScript;
    public infantry_formations formationScript;
    public infantry_attack attackScript;

    id_dictionary id_table;

    public float shootSpeed;
    public int damage;

    public GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        id_table = GameObject.Find("CustomEventSystem").GetComponent<id_dictionary>(); //holds the id's of all active gameobjects
        id_table.addObject(gameObject);

        agentScript = gameObject.AddComponent<Agent>();
        agentScript.maxSpeed = 5.0f;

        followScript = gameObject.AddComponent<infantry_follow>();
        followScript.enabled = false;

        formationScript = gameObject.AddComponent<infantry_formations>();
        formationScript.enabled = false;

        attackScript = gameObject.AddComponent<infantry_attack>();
        attackScript.enabled = false;
    }

    private void OnDestroy()
    {
        parent_object.GetComponent<Fireteam>().fireteam.Remove(gameObject);

        if (parent_object.GetComponent<Fireteam>().fireteam.Count <= 0)
        {
            Destroy(parent_object);
        }

        if (selectionObject != null)
        {
            Destroy(selectionObject);
        }
    }
}
