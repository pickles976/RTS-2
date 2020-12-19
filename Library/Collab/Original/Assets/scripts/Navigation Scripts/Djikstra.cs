using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Djikstra
{

    //Row and Column depend on grid size
    public int COL;
    public int ROW;

    struct cell
    {
        //Row and Column index of its parent
        //Note that 0 <= i <= ROW - 1 & 0 <= j <= COL - 1
        public int parent_i, parent_j; //indices of parent cell
        public int row, col; //indices in cell array
        public int distance; //distance from the source
    }

    //Utility function to check whether a given cell is valid or not
    bool isValid(int row, int col)
    {
        //returns true if row# and col# is within range
        return (row >= 0) && (row < ROW) &&
               (col >= 0) && (col < COL);
    }

    //utility function to check whether the given cell is blocked or not
    bool isUnBlocked(int[,] grid, int row, int col)
    {
        //returns true if the cell is not clocked else false
        if (grid[row, col] == Int32.MaxValue)
        {
            return (false);
        }
        else
        {
            return (true);
        }
    }

    //Utility function to check whether destination cell has been reached or not
    bool isDestination(int row, int col, Pair<int, int> dest)
    {
        if (row == dest.first && col == dest.second)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    //generate djikstra grid on a given obstacle grid
    public int[,] generate_djikstra_grid(int col_size,int row_size, int[,] obstacle_grid, Pair<int, int> dest)
    {

        ROW = row_size;
        COL = col_size;
        int i;
        int j;

        int[,] dijkstraGrid = obstacle_grid; //give the djikstra grid blocked spaces

        //check if the destination is out of range
        if (isValid(dest.first, dest.second) == false)
        {
            Debug.Log("Dijkstra destination is invalid");
            return null;
        }

        //Destination is blocked
        if (isUnBlocked(obstacle_grid, dest.second, dest.first) == false)
        {
            Debug.Log("Dijkstra destination is blocked");
            return null;
        }

        //FLOOD FILL FROM THE END POINT

        //declare a 2D array of cell structures to hold the details of that cell
        var cellDetails = new cell[ROW, COL];

        //initialize with default values for cellDetails
        for (i = 0; i < ROW; i++)
        {
            for (j = 0; j < COL; j++) { 
                cellDetails[i, j].parent_i = -1;
                cellDetails[i, j].parent_j = -1;
                cellDetails[i, j].distance = -1;
                cellDetails[i, j].row = i;
                cellDetails[i, j].col = j;
        }
        }

        //initialize destination cell
        cellDetails[dest.first, dest.second].distance = 0;

        Queue<cell> toVisit = new Queue<cell>(); //FIFO STRUCTURE

        i = dest.first; //row
        j = dest.second; //col

        //SET PARENTS AND ADD FIRST FOUR NEIGHBOURS TO QUEUE
        //WORKING
        if (j + 1 < ROW) //check for within bounds
        {

                cellDetails[dest.first, dest.second + 1].parent_i = i;
                cellDetails[dest.first, dest.second + 1].parent_j = j;
                toVisit.Enqueue(cellDetails[dest.first, dest.second + 1]); //right

        }

        if (i > 0)
        {

                cellDetails[dest.first - 1, dest.second].parent_i = i;
                cellDetails[dest.first - 1, dest.second].parent_j = j;
                toVisit.Enqueue(cellDetails[dest.first - 1, dest.second]); //down\

        }

        if (j > 0)
        {

                cellDetails[dest.first, dest.second - 1].parent_i = i;
                cellDetails[dest.first, dest.second - 1].parent_j = j;
                toVisit.Enqueue(cellDetails[dest.first, dest.second - 1]); //left

        }

        if (i + 1 < COL)
        {
                //Debug.Log("ROW = " + ROW + " COL = " + COL);
                //Debug.Log("dest.first = " + (dest.first + 1) + " dest.second = " + dest.second);
                cellDetails[dest.first + 1, dest.second].parent_i = i;
                cellDetails[dest.first + 1, dest.second].parent_j = j;
                toVisit.Enqueue(cellDetails[dest.first + 1, dest.second]); //up

        }

        while (toVisit.Count > 0)
        {
            cell temp_cell = toVisit.Dequeue(); //pull cell of the QUEUE

            //check if the destination is out of range
            if (isValid(temp_cell.row, temp_cell.col))
            {
                    if (cellDetails[temp_cell.row, temp_cell.col].distance == -1)
                    {

                    //Check if destination is blocked
                    if (isUnBlocked(obstacle_grid,temp_cell.col, temp_cell.row))
                    {
                            //grab the parent's indices
                            int parent_i = cellDetails[temp_cell.row, temp_cell.col].parent_i;
                            int parent_j = cellDetails[temp_cell.row, temp_cell.col].parent_j;

                        //Debug.Log(string.Format("i: {0}, j: {1}, p_i: {2}, p_j: {3}", temp_cell.row, temp_cell.col, parent_i, parent_j));

                            //calculate distance from parents
                            cellDetails[temp_cell.row, temp_cell.col].distance = cellDetails[parent_i,parent_j].distance + 1;

                            //change to the indices of the currently selected cell
                            i = temp_cell.row;
                            j = temp_cell.col;


                        if (j + 1 < COL) //check for within bounds
                        {
                            //check if it has been unvisited (parent set to -1,-1)
                            if ((cellDetails[i, j + 1].parent_i == -1) && (cellDetails[i, j + 1].parent_j == -1)){
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

                        if (i + 1 < ROW)
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
                            cellDetails[temp_cell.row, temp_cell.col].distance = Int32.MaxValue;
                        }
                    }//end if uninitialized
            }//end if isValid


        }//end while

        //transfer distance values to the Dijkstra grid
        for(i = 0; i < ROW; i++)
        {
            //String s = "";
            for(j = 0; j < COL; j++)
            {

                    if (dijkstraGrid[j, i] != Int32.MaxValue)
                    {
                        dijkstraGrid[j, i] = cellDetails[i, j].distance;
                        //s = s + cellDetails[i, j].distance;
                    }


                /*
                else
                {
                    s = s + "X";
                }
                */
            }
            //Debug.Log(s); // debug print
        }

        return dijkstraGrid;

    }
}
