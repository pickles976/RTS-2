using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class custom_grid
{
    //transforms global coordinates into cell positions based on 
    //grid size

    public float cellSize;

    //transforms world position into grid indices
    public Tuple<int,int> worldToCell(Vector3 worldPos)
    {
       int xCell = (int)Mathf.Floor(worldPos.x / cellSize);
       int zCell = (int)Mathf.Floor(worldPos.z / cellSize);

       return new Tuple<int,int>(xCell,zCell);
    }

    //turns grid indices into approximate world coordinates
    public Vector3 cellToWorld(int xCell,int zCell)
    {
        Vector3 vector = new Vector3(0.0f,0.0f,0.0f);

        vector.x = xCell * cellSize;
        vector.z = zCell * cellSize;

        return vector;
    }

}
