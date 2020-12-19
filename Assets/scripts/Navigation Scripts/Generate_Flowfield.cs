using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate_Flowfield
{
    public Vector3[,] generate_flowfield(int row,int col,int[,] dijkstra_grid,int xOff,int zOff,float cellSize)
    {

        int row_size = row;
        int col_size = col;

        int [,] dijkstra = dijkstra_grid;
        Vector3[,] flowfield = new Vector3[row_size, col_size];

        for (int j = 0; j < col_size; j++)
        {
            for(int i = 0; i < row_size; i++)
            {
                int min = Int32.MaxValue;
                int j_dest = -1; //the indices of the cell with the smallest cost value
                int i_dest = -1; //set these to -1 so we know if they are not set

                //Debug.Log(dijkstra.Length);
                //Debug.Log(i);
                //Debug.Log(j);

                //(i,j+1) EAST
                if (j + 1 < col_size)
                {
                    if ((dijkstra[i,j+1] < min) && (dijkstra[i, j + 1] != -1))
                    {
                        min = dijkstra[i, j + 1];
                        i_dest = i;
                        j_dest = j + 1;
                    }
                }

                //(i-1,j+1) SOUTH-EAST
                if ((j + 1 < col_size) && (i > 0))
                {
                    if ((dijkstra[i - 1, j + 1] < min) && (dijkstra[i - 1, j + 1] != -1))
                    {
                        min = dijkstra[i - 1, j + 1];
                        i_dest = i - 1;
                        j_dest = j + 1;
                    }
                }

                //(i,j+1) SOUTH
                if (i > 0)
                {
                    if ((dijkstra[i - 1, j] < min) && (dijkstra[i - 1, j] != -1))
                    {
                        min = dijkstra[i - 1, j];
                        i_dest = i - 1;
                        j_dest = j;
                    }
                }

                //(i-1,j-1) SOUTH-WEST
                if ((j > 0) && (i > 0))
                {
                    if ((dijkstra[i - 1, j - 1] < min) && (dijkstra[i - 1, j - 1] != -1))
                    {
                        min = dijkstra[i - 1, j - 1];
                        i_dest = i - 1;
                        j_dest = j - 1;
                    }
                }

                //(i,j+1) WEST
                if (j > 0 )
                {
                    if ((dijkstra[i, j - 1] < min) && (dijkstra[i, j - 1] != -1))
                    {
                        min = dijkstra[i, j - 1];
                        i_dest = i;
                        j_dest = j - 1;
                    }
                }

                //(i+1,j-1) NORTH-WEST
                if ((j > 0) && (i + 1 < row_size))
                {
                    if ((dijkstra[i + 1, j - 1] < min) && (dijkstra[i + 1, j - 1] != -1))
                    {
                        min = dijkstra[i + 1, j - 1];
                        i_dest = i + 1;
                        j_dest = j - 1;
                    }
                }

                //(i,j+1) NORTH
                if (i + 1 < row_size)
                {
                    if ((dijkstra[i + 1, j] < min) && (dijkstra[i + 1, j] != -1))
                    {
                        min = dijkstra[i + 1, j];
                        i_dest = i + 1;
                        j_dest = j;
                    }
                }

                //(i+1,j-1) NORTH-EAST
                if ((j + 1 < col_size) && (i + 1 < row_size))
                {
                    if ((dijkstra[i + 1, j + 1] < min) && (dijkstra[i + 1, j + 1] != -1))
                    {
                        min = dijkstra[i + 1, j + 1];
                        i_dest = i + 1;
                        j_dest = j + 1;
                    }
                }

                Vector3 field = new Vector3();

                field.y = 0.0f;
                field.x = (float)(i_dest - i);
                field.z = (float)(j_dest - j);

                if((i_dest == -1) || (j_dest == -1))
                {
                    field = new Vector3(0, 0, 0);
                }

                Color grad = new Vector4(0.01f * dijkstra[i, j], 0.0f,0.0f, 1);

                flowfield[i, j] = field/(field.magnitude); //normalize vector
                //DrawArrow.ForDebug(new Vector3((float)(i + xOff)*cellSize,0.5f,(float)(j + zOff)*cellSize), field/field.magnitude, grad);

            }//end for j
        }//end for i

        return flowfield;

    }//end function

    


}
