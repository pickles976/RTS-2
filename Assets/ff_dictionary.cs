using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this holds a dictionary of flowfields, which our Agents (or groups) use to query their flowfields
public class ff_dictionary : MonoBehaviour
{

    Dictionary<int,Flowfield> ff_table;

    // Start is called before the first frame update
    void Start()
    {
        ff_table = new Dictionary<int, Flowfield>();
    }

    //add a flowfield id pair to the table
    public void addFF(int id, Flowfield flowField)
    {
        if (ff_table.ContainsKey(id))
        {
            ff_table[id] = flowField;
        }
        else
        {
            ff_table.Add(id, flowField);
        }
    }

    //delete flowfield by id
    public void deleteFF(int id)
    {
        ff_table.Remove(id);
    }

    //
    public bool contains(int id)
    {
        return ff_table.ContainsKey(id);
    }

    //based on your position and your linked flowfield, this method returns the vector you need to apply
    public Vector3 getFFVector(GameObject go)
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

        Flowfield ff = ff_table[id];

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
        switch (ff.type)
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

        if (go.GetComponent<flowfield_pathfinding>().show_ff_debug) //if we are in debug mode
        {
            for (int j = 0; j < ff.col_length; j++)
            {
                for (int i = 0; i < ff.row_length; i++)
                {
                    if ((xCell == i) && (zCell == j))
                    {
                        DrawArrow.ForDebug(new Vector3((float)(i + ff.xOff + 0.5f) * subGrid.cellSize, 6.0f, (float)(j + ff.zOff + 0.5f) * subGrid.cellSize), ff.flowfield[i, j], Color.blue);
                    }
                    else
                    {
                        DrawArrow.ForDebug(new Vector3((float)(i + ff.xOff + 0.5f) * subGrid.cellSize, 6.0f, (float)(j + ff.zOff + 0.5f) * subGrid.cellSize), ff.flowfield[i, j], Color.blue);
                    }
                }

            }
        }

        return ff_table[id].flowfield[xCell, zCell];
    }

}
