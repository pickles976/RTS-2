using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resource_generation : MonoBehaviour
{

    int amount;
    float frequency;
    float timer;
    public Team_dictionary td;
    public structure_behavior sb;

    public oil_deposit_script oil_deposit;

    public void setResources(int amt)
    {
        this.amount = amt;
    }

    private void Start()
    {
        td = GameObject.FindGameObjectWithTag("CustomEventSystem").GetComponent<Team_dictionary>();
        frequency = 5.0f;
        timer = 0.0f;

        //look for oil deposit, and attach to it.
        //update the waypoint
            Ray ray = new Ray(transform.position, Vector3.down); //raycast from previous mouse pointer position
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50000.0f,(int)(1<<15))) //if we hit the ground
            {
                oil_deposit = hit.transform.gameObject.GetComponent<oil_deposit_script>();
            }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > frequency)
        {
            timer = 0.0f;
            //subtract oil from well
            td.getTeamData(sb.team).oil_amount += oil_deposit.extractOil(amount);
        }
    }
}
