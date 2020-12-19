using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//abstract class that lets us define multiple formations with just an array of Vector3's
public class FormationPattern : MonoBehaviour
{

    //get the position of a given slot index
    public virtual Vector3 GetSlotLocation(int slotIndex)
    {
        return Vector3.zero;
    }

}
