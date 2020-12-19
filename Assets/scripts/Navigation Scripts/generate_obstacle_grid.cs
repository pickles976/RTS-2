using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generate_obstacle_grid
{

    custom_grid grid;
    int xOffset;
    int zOffset;

    RaycastHit hit; //raycast return variable
    GameObject go;	//keeping track of current game object

    float radius = 2.0f; //radius of capsulecast

    //This function will generate a 2D array of either NULL or MAXINT depending on whether there are
    //obstacles in the way or not.
    public int[,] Generate_grid(float cellSize, int row_size,int col_size,int xOff,int zOff)
    {
        xOffset = xOff;
        zOffset = zOff;
        grid = new custom_grid();
        grid.cellSize = cellSize;

        int[,] obstacle_grid = new int[row_size, col_size]; //create an empty grid to load obstacle values into

    
        //col is col index, row is row index
        for (int col = 0; col < col_size; col++)
        {
            for (int row = 0; row < row_size; row++)
            {

                Vector3 half_offset = (Vector3.right * 0.5f * cellSize) + (Vector3.forward * 0.5f * cellSize);
                //create a new ray from 100 units up pointing down towards the map
                Ray ray = new Ray(grid.cellToWorld(row + xOffset, col + zOffset) + half_offset + (1000.0f * Vector3.up),Vector3.down);

                Vector3 origin = grid.cellToWorld(row + xOffset, col + zOffset) + half_offset + (1000.0f * Vector3.up);

                //if (Physics.Raycast(ray, out hit, 50000.0f,1<<10)) //check a raycast out to infinity
                if (Physics.SphereCast(origin,radius, Vector3.down,out hit, 50000.0f, (1 << 10) | (1<<14)))
                {

                    //return the GameObject we hit
                    go = hit.transform.gameObject;
                    obstacle_grid[row, col] = Int32.MaxValue; //Max value represents impassable object

                }
                else if (Physics.Raycast(origin, -Vector3.up, out hit, 50000.0f, (1 << 8))) //check if it is not an impassable slope
                {
                    if (Vector3.Angle(Vector3.up,hit.normal) > 5.0f) 
                    {
                        obstacle_grid[row, col] = Int32.MaxValue; //Max value represents impassable object
                    }
                    else if (hit.point.y < 3.8f)
                    {
                        obstacle_grid[row, col] = Int32.MaxValue; //Max value represents impassable object
                    }
                }
            }
        }

            return obstacle_grid;

    }

    //PERFORM BREADTH-FIRST SEARCH TO FIND NEAREST UNBLOCKED CELL
    public Pair<int,int> find_nearest_unblocked(int[,] grid,int dest_i,int dest_j,int row_length,int col_length)
    {
        if (grid[dest_i,dest_j] == Int32.MaxValue)
        {
            //declare a 2D array of cell structures to hold the details of that cell
            var cellDetails = new cell[row_length, col_length];
            int i;
            int j;

            //initialize with default values for cellDetails
            for (i = 0; i < row_length; i++)
            {
                for (j = 0; j < col_length; j++)
                {
                    cellDetails[i, j].parent_i = -1;
                    cellDetails[i, j].parent_j = -1;
                    cellDetails[i, j].row = i;
                    cellDetails[i, j].col = j;
                    cellDetails[i, j].distance = -1;
                }
            }

            i = 0;
            j = 0;

            Pair<int, int> dest = new Pair<int, int>(dest_i, dest_j);

            Queue<cell> toVisit = new Queue<cell>(); //FIFO STRUCTURE

            //SET PARENTS AND ADD FIRST FOUR NEIGHBOURS TO QUEUE
            //WORKING
            if (dest_j + 1 < row_length) //check for within bounds
            {
                cellDetails[dest.first, dest.second + 1].parent_i = dest_i;
                cellDetails[dest.first, dest.second + 1].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.first, dest.second + 1]); //right
            }

            if (dest_i > 0)
            {
                cellDetails[dest.first - 1, dest.second].parent_i = dest_i;
                cellDetails[dest.first - 1, dest.second].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.first - 1, dest.second]); //down
            }

            if (dest_j > 0)
            {
                cellDetails[dest.first, dest.second - 1].parent_i = dest_i;
                cellDetails[dest.first, dest.second - 1].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.first, dest.second - 1]); //left
            }

            if (dest_i + 1 < col_length)
            {
                cellDetails[dest.first + 1, dest.second].parent_i = dest_i;
                cellDetails[dest.first + 1, dest.second].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.first + 1, dest.second]); //up
            }

            while (toVisit.Count > 0)
            {
                cell temp_cell = toVisit.Dequeue(); //pull cell of the QUEUE

                    if (cellDetails[temp_cell.row, temp_cell.col].distance == -1)
                    {

                        //Destination is blocked
                        if (grid[temp_cell.row,temp_cell.col] == int.MaxValue)
                        {

                            //change to the indices of the currently selected cell
                            i = temp_cell.row;
                            j = temp_cell.col;


                            if (j + 1 < col_length) //check for within bounds
                            {
                                //check if it has been unvisited (parent set to -1,-1)
                                if ((cellDetails[i, j + 1].parent_i == -1) && (cellDetails[i, j + 1].parent_j == -1))
                                {
                                    cellDetails[i, j + 1].parent_i = i;
                                    cellDetails[i, j + 1].parent_j = j;
                                    toVisit.Enqueue(cellDetails[i, j + 1]); //right
                                }
                            }

                            if (i > 0)
                            {
                                if ((cellDetails[i - 1, j].parent_i == -1) && (cellDetails[i - 1, j].parent_j == -1))
                                {
                                    cellDetails[i - 1, j].parent_i = i;
                                    cellDetails[i - 1, j].parent_j = j;
                                    toVisit.Enqueue(cellDetails[i - 1, j]); //down
                                }
                            }

                            if (j > 0)
                            {
                                if ((cellDetails[i, j - 1].parent_i == -1) && (cellDetails[i, j - 1].parent_j == -1))
                                {
                                    cellDetails[i, j - 1].parent_i = i;
                                    cellDetails[i, j - 1].parent_j = j;
                                    toVisit.Enqueue(cellDetails[i, j - 1]); //left
                                }
                            }

                            if (i + 1 < row_length)
                            {
                                if ((cellDetails[i + 1, j].parent_i == -1) && (cellDetails[i + 1, j].parent_j == -1))
                                {
                                    cellDetails[i + 1, j].parent_i = i;
                                    cellDetails[i + 1, j].parent_j = j;
                                    toVisit.Enqueue(cellDetails[i + 1, j]); //up
                                }
                            }

                        }
                        else
                        {
                            //calculate distance from parents
                            return new Pair<int,int>(temp_cell.row,temp_cell.col);
                        }
                    }//end if uninitialized


            }//end while


        }

            return new Pair<int, int>(dest_i,dest_j);
        
    }

    struct cell
    {
        //Row and Column index of its parent
        //Note that 0 <= i <= ROW - 1 & 0 <= j <= COL - 1
        public int parent_i, parent_j; //indices of parent cell
        public int row, col; //indices in cell array
        public int distance;
    }


}
