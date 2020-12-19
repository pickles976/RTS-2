using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement_group_dictionary : MonoBehaviour
{

    //foreign objects should only access the removeFromGroup() and addToGroup() functions
    //these two functions should handle all cases

    //dictionary links target gameobject, to a list of movement group objects 
    Dictionary<GameObject, List<movement_group>> movement_group_table = new Dictionary<GameObject, List<movement_group>>();

    float lookdistance = 50.0f;

    //add a gameObject to the group specified
    public void addToGroup(GameObject target, GameObject member)
    {
        if (containsGroup(target))//check if there is a list with the specified target
        {
            //Debug.Log("A group with target: " + target + " exists!");

            //perform a spherecast on all nearby units to see if any of them are already in a group in this list
            Collider[] hitColliders = Physics.OverlapSphere(member.transform.position, lookdistance,(1<<9));
            foreach(Collider c in hitColliders)
            {

                //Debug.Log("Colliders hit!");

                if (insertIfMemberHasTarget(target, c.gameObject, member) == true)//check if any of our neighbors have the same target
                {
                    return;
                }
            }
            addGroup(target, member); //add a new group with self as the leader
        }
        else
        {
            addGroup(target, member); // add a new group with self as the leader
        }
    }

    //remove gameobject from movement group
    public void removeFromGroup(GameObject target, GameObject member)
    {
        if (movement_group_table.ContainsKey(target)) //check if a list for our target exists
        {
            foreach (movement_group mg in movement_group_table[target]) //loop through all movement groups in the list
            {
                if (mg.containsMember(member)) //if we are in this group
                {
                    mg.removeMember(member); //remove us and return

                    if (mg.isEmpty()) //if this movement group is now empty
                    {
                        movement_group_table[target].Remove(mg); //remove it from the list

                        if (movement_group_table[target].Count == 0) //if the list is now empty
                        {
                            movement_group_table.Remove(target); //remove the list altogether
                        }

                    }

                    return;
                }
            }
        }
    }


    //find parent ground
    public movement_group getParentGroup(GameObject target, GameObject member)
    {
        if (movement_group_table.ContainsKey(target)) //check if a list for our target exists
        {
            foreach (movement_group mg in movement_group_table[target]) //loop through all movement groups in the list
            {
                if (mg.containsMember(member)) //if we are in this group
                {
                    return mg;
                }
            }
        }

        return new movement_group();
    }


//========================================================================================================================//


    //add a new group for the specified target
    void addGroup(GameObject target,GameObject leader)
    {
        if (movement_group_table.ContainsKey(target)) //if we have a list for the target
        {
            movement_group new_group = new movement_group(); //add a new movement group to the list
            new_group.addMember(leader);
            new_group.leader = leader;
            movement_group_table[target].Add(new_group); 
        }else{
            List<movement_group> mg_list = new List<movement_group>(); //create a new list and add a movement group to it
            movement_group new_group = new movement_group();
            new_group.addMember(leader);
            new_group.leader = leader;
            mg_list.Add(new_group);
            movement_group_table.Add(target,mg_list); //put the list into the table
        }
    }

    //look for a specific gameobject with a specified target
    bool insertIfMemberHasTarget(GameObject target,GameObject member,GameObject new_member)
    {
        if (movement_group_table.ContainsKey(target)) //see if there is a movement group list for this target
        {
            foreach (movement_group mg in movement_group_table[target]) //iterate through the list
            {
                foreach(GameObject unit in mg.group) //look through every unit in the group
                {
                    if (unit == member) //check if the member is in the group
                    {
                            mg.addMember(new_member);
                            new_member.GetComponent<seek_target>().mg = mg; //set the object's movement group;
                            return true; //return true
                    }
                }
            }
        }

        return false;
    }

    //tell us if a movement group exists for this target
    bool containsGroup(GameObject target)
    {
        return movement_group_table.ContainsKey(target);
    }

    
}
