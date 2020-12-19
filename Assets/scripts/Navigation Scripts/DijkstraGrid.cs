using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DijkstraGrid
{

    public int[,] grid;
    public int row_length;
    public int col_length;
    public int xOff;
    public int zOff;
    public int type;

    //based on your position and your linked flowfield, this method returns the vector you need to apply
    public int getValuefromObject(GameObject go)
    {
        /*    
             * Q3 | Q0
             * -------
             * Q2 | Q1
             * 
             *    +X
             * -Z    +Z
             *    -X
             */

        int id = go.GetInstanceID();

        custom_grid subGrid = go.GetComponent<flowfield_pathfinding>().subGrid;
        int grid_ratio = go.GetComponent<flowfield_pathfinding>().grid_ratio;

        int xCell = subGrid.worldToCell(go.transform.position).Item1 % grid_ratio; //10 is the number of cells per supergrid
        int zCell = subGrid.worldToCell(go.transform.position).Item2 % grid_ratio;

        if (xCell < 0) //compensate for negatives
        {
            xCell = grid_ratio + xCell;
        }

        if (zCell < 0)
        {
            zCell = grid_ratio + zCell;
        }

        //we have to transform our world grid coordinates (pos and neg) to the coordinates of the flowfield (pos only)
        switch (type)
        {
            case 0:
                break;

            case 1:
                xCell = xCell + grid_ratio;
                break;

            case 2:
                xCell = xCell + grid_ratio;
                zCell = zCell + grid_ratio;
                break;

            case 3:
                zCell = zCell + grid_ratio;
                break;

        }

        //DEBUG STUFF

        return grid[xCell, zCell];
    }

}
