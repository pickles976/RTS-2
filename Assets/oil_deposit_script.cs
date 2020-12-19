using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oil_deposit_script : MonoBehaviour
{

    public int oil_amount;

    public int extractOil(int amt)
    {
        if (oil_amount > 0)
        {
            if (oil_amount > amt)
            {
                oil_amount -= amt;
                return amt;
            }
            else
            {
                int temp = oil_amount;
                oil_amount = 0;
                return temp;
            }
        }

        return 0;

    }
}
