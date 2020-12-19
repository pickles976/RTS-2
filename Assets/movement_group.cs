using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement_group
{

    public List<GameObject> group = new List<GameObject>(); //list of units in the group
    //Dictionary<int, bool> isLeader = new Dictionary<int, bool>(); //maps ID's to whether or not the unit is a leader
    public GameObject leader;

    public float maxSpeed = float.MaxValue; //

    //add a new member to the group
    public void addMember(GameObject member)
    {
        if(leader == null) //if there is no leader
        {
            leader = member;
            //set leader flag
            member.GetComponent<unit_behavior>().isMgLeader = true;
        }

        if(member.GetComponent<Agent>().maxSpeed < maxSpeed) //check who the slowest member of the movement group is
        {
            maxSpeed = member.GetComponent<Agent>().maxSpeed; //set everyone to match the slowest member's pace
        }

        group.Add(member);
    }

    //remove a unit from the group and assign a new leader if the previous leader is removed
    public void removeMember(GameObject member)
    {
        if (member == leader) //if the group leader is the object we are removing
        {
            group.Remove(member); //remove the leader from the list of units
            member.GetComponent<unit_behavior>().isMgLeader = false; //set unit leader flag to FALSE

            if (group.Count > 0)
            {
                if (group[0] != null)
                {
                    leader = group[0]; //re-add the new top member as the leader
                                       //update the leader's behavior
                    leader.GetComponent<unit_behavior>().isMgLeader = true; //set unit leader flag to TRUE
                }
            }
            else
            {
                leader = null;
            }
        }
        else
        {
            group.Remove(member);
        }

        member.GetComponent<Agent>().speedReset();
    }

    public bool containsMember(GameObject member)
    {
        return group.Contains(member);
    }

    public bool isEmpty()
    {
        return (!(group.Count > 0));
    }
}
