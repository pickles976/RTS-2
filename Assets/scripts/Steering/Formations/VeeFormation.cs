using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//classic V formation
public class VeeFormation : FormationPattern
{

    Vector3[] formation;

    float spacing = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        formation = new Vector3[6];

        formation[0] = Vector3.zero;

        for (int i = 1; i < 6; i++) {

            float offset = (i + 1) / 2;

            formation[i] = new Vector3(spacing * offset, 0, spacing * offset * Mathf.Pow(-1, i));
        }
    }

    //get the relative position for this formation
    public override Vector3 GetSlotLocation(int slotIndex)
    {
        return formation[slotIndex];
    }
}
