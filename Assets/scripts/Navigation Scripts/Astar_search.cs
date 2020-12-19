using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Pair.cs;

public class Astar_search
{
    //NOTE: "new Pair" is replace with "new Pair(i,j)"

    //Row and Column depend on grid size
    public int COL;
    public int ROW;
    double FLT_MAX = float.MaxValue;

    //no typedef, create class for Pair, use nested for pPair
    // Creating a shortcut for int, int pair type 
    //typedef pair<int, int> Pair;

    // Creating a shortcut for pair<int, pair<int, int>> type 
    //typedef pair<double, pair<int, int>> pPair;

        //structure for each cell
    struct cell
    {
        //Row and Column index of its parent
        //Note that 0 <= i <= ROW - 1 & 0 <= j <= COL - 1
        public int parent_i, parent_j;
        //f = g + h (heuristic)
        public double f, g, h;
    }

    //Utility function to check whether a given cell is valid or not
    bool isValid(int row,int col)
    {
        //returns true if row# and col# is within range
        return (row >= 0) && (row < ROW) &&
               (col >= 0) && (col < COL);
    }

    //utility function to check whether the given cell is blocked or not
    bool isUnBlocked(int[,] grid,int row,int col)
    {
        //returns true if the cell is not clocked else false
        if (grid[row,col] == 1)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    //Utility function to check whether destination cell has been reached or not
    bool isDestination(int row, int col, Pair<int,int> dest)
    {
        if ((row == dest.first) && (col == dest.second))
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    //A Utility function to calculate the 'h' heuristics
    double calculateHValue(int row,int col,Pair<int, int> dest)
    {
        //pythagorean theorem straight shot to destination
        return ((double)Math.Sqrt((row - dest.first) * (row - dest.first) + (col - dest.second) * (col - dest.second)));
    }

    //A utility function to trace the path from the source to the destination
    void tracePath(cell[,] cellDetails,Pair<int, int> dest,ref Stack<Pair<int,int>> path)
    {
        //Debug.Log("\n The path is");
        int row = dest.first;
        int col = dest.second;

        Stack<Pair<int, int>> Path = new Stack<Pair<int, int>>(); //LIFO order

        while(!(cellDetails[row,col].parent_i == row && cellDetails[row,col].parent_j ==col ))
        {
            Path.Push(new Pair<int,int>(row, col));
            int temp_row = cellDetails[row,col].parent_i;
            int temp_col = cellDetails[row,col].parent_j;
            row = temp_row;
            col = temp_col;
        }

        Path.Push(new Pair<int,int>(row, col));

        path = Path;
        /*
        while (!(Path.Count == 0)) //while stack not empty
        {
            Pair<int,int> p = Path.Pop(); //get item off top of stack
            Debug.Log(string.Format("-> ({0},{1}) ", p.first, p.second));
        }
        */

        return;

    }

    //A STAR ALGO
    //=================================================================================//

    //A function given to find the shortest path between
    //a given source cell to a destination cell according to
    //the A* search algorithm
    public void aStarSearch(int[,] grid,Pair<int, int> src, Pair<int, int> dest,int col_size,int row_size,ref Stack<Pair<int,int>> path)
    {

        ROW = col_size;
        COL = row_size;

        //check if the source is out of range
        if(isValid(src.first,src.second) == false)
        {
            //Debug.Log(row_size);
            //Debug.Log(col_size);
            //Debug.Log(src.first);
            //Debug.Log(src.second);
            Debug.Log("Source is invalid");
            return;
        }

        //check if the destination is out of range
        if(isValid(dest.first,dest.second) == false)
        {
            //Debug.Log(row_size);
            //Debug.Log(col_size);
            //Debug.Log(dest.first);
            //Debug.Log(dest.second);
            Debug.Log("Destination is invalid");
            return;
        }

        //Either the source or the destination is blocked
        if(isUnBlocked(grid,src.first,src.second) == false || isUnBlocked(grid,dest.first,dest.second) == false)
        {
            Debug.Log("Source or destination is blocked");
            return;
        }

        //If the destination cell is the same as the source cell
        if(isDestination(src.first,src.second,dest) == true)
        {
            Debug.Log("We are already at the destination");
            return;
        }

        //Create a closed list and initialize it to false which means that no cell has been included yet
        //This closed list is implemented as a boolean 2D array
        var closedList = new bool[ROW,COL];
        for(int k = 0; k < ROW; k++) //set whole thing to false
        {
            for(int l = 0; l < COL; l++)
            {
                closedList[k,l] = false;
            }
        }

        //declare a 2D array of cell structures to hold the details of that cell
        var cellDetails = new cell[ROW,COL];

        int i, j;

        //initialize with default values for cellDetails
        for (i = 0; i < ROW; i++)
        {
            for (j = 0; j < COL; j++)
            {
                cellDetails[i,j].f = FLT_MAX;
                cellDetails[i,j].g = FLT_MAX;
                cellDetails[i,j].h = FLT_MAX;
                cellDetails[i,j].parent_i = -1;
                cellDetails[i,j].parent_j = -1;
            }
        }

        // Initialising the parameters of the starting node 
        i = src.first;
        j = src.second;
        cellDetails[i,j].f = 0.0;
        cellDetails[i,j].g = 0.0;
        cellDetails[i,j].h = 0.0;
        cellDetails[i,j].parent_i = i;
        cellDetails[i,j].parent_j = j;

        /* 
        Create an open list having information as- 
        <f, <i, j>> 
        where f = g + h, 
        and i, j are the row and column index of that cell 
        Note that 0 <= i <= ROW-1 & 0 <= j <= COL-1 
        This open list is implenented as a set of pair of pair.*/
        //set<pPair> openList; 

        SortedSet<Pair<double, Pair<int, int>>> openList = new SortedSet<Pair<double, Pair<int, int>>>(); //recall that our pair can take any values, including a pair

        // Put the starting cell on the open list and set its 
        // 'f' as 0 
        openList.Add(new Pair<double,Pair<int,int>>(0.0, new Pair<int,int>(i, j)));

        // We set this boolean value as false as initially 
        // the destination is not reached. 
        bool foundDest = false;

        //MAIN SEARCH LOOP
        //=================================================================================//

        while (!(openList.Count == 0))
        {
            Pair<double,Pair<int,int>> p = openList.Min; //get the first cell in the open List (ordered by double f)

            // Remove this vertex from the open list 
            openList.Remove(openList.Min);

            // Add this vertex to the closed list 
            i = p.second.first;
            j = p.second.second;
            closedList[i,j] = true;

            /* 
             Generating all the 8 successor of this cell 

                 N.W   N   N.E 
                   \   |   / 
                    \  |  / 
                 W----Cell----E 
                      / | \ 
                    /   |  \ 
                 S.W    S   S.E 

             Cell-->Popped Cell (i, j) 
             N -->  North       (i-1, j) 
             S -->  South       (i+1, j) 
             E -->  East        (i, j+1) 
             W -->  West           (i, j-1) 
             N.E--> North-East  (i-1, j+1) 
             N.W--> North-West  (i-1, j-1) 
             S.E--> South-East  (i+1, j+1) 
             S.W--> South-West  (i+1, j-1)*/

            // To store the 'g', 'h' and 'f' of the 8 successors 
            double gNew, hNew, fNew;

            //----------- 1st Successor (North) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i - 1, j) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i - 1, j, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i - 1,j].parent_i = i;
                    cellDetails[i - 1,j].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest,ref path);
                    foundDest = true;
                    return;
                }
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i - 1,j] == false &&
                         isUnBlocked(grid, i - 1, j) == true)
                {
                    gNew = cellDetails[i,j].g + 1.0;
                    hNew = calculateHValue(i - 1, j, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i - 1,j].f == FLT_MAX ||
                            cellDetails[i - 1,j].f > fNew)
                    {
                        openList.Add(new Pair<double,Pair<int,int>>(fNew,
                                                   new Pair<int,int>(i - 1, j)));

                        // Update the details of this cell 
                        cellDetails[i - 1,j].f = fNew;
                        cellDetails[i - 1,j].g = gNew;
                        cellDetails[i - 1,j].h = hNew;
                        cellDetails[i - 1,j].parent_i = i;
                        cellDetails[i - 1,j].parent_j = j;
                    }
                }
            }

            //----------- 2nd Successor (South) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i + 1, j) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i + 1, j, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i + 1,j].parent_i = i;
                    cellDetails[i + 1,j].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest, ref path);
                    foundDest = true;
                    return;
                }
                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i + 1,j] == false &&
                         isUnBlocked(grid, i + 1, j) == true)
                {
                    gNew = cellDetails[i,j].g + 1.0;
                    hNew = calculateHValue(i + 1, j, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i + 1,j].f == FLT_MAX ||
                            cellDetails[i + 1,j].f > fNew)
                    {
                        openList.Add(new Pair<double, Pair<int, int>>(fNew, new Pair<int,int>(i + 1, j)));
                        // Update the details of this cell 
                        cellDetails[i + 1,j].f = fNew;
                        cellDetails[i + 1,j].g = gNew;
                        cellDetails[i + 1,j].h = hNew;
                        cellDetails[i + 1,j].parent_i = i;
                        cellDetails[i + 1,j].parent_j = j;
                    }
                }
            }

            //----------- 3rd Successor (East) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i, j + 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i, j + 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i,j + 1].parent_i = i;
                    cellDetails[i,j + 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest, ref path);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i,j + 1] == false &&
                         isUnBlocked(grid, i, j + 1) == true)
                {
                    gNew = cellDetails[i,j].g + 1.0;
                    hNew = calculateHValue(i, j + 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i,j + 1].f == FLT_MAX ||
                            cellDetails[i,j + 1].f > fNew)
                    {
                        openList.Add(new Pair<double, Pair<int, int>>(fNew,
                                            new Pair<int,int>(i, j + 1)));

                        // Update the details of this cell 
                        cellDetails[i,j + 1].f = fNew;
                        cellDetails[i,j + 1].g = gNew;
                        cellDetails[i,j + 1].h = hNew;
                        cellDetails[i,j + 1].parent_i = i;
                        cellDetails[i,j + 1].parent_j = j;
                    }
                }
            }

            //----------- 4th Successor (West) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i, j - 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i, j - 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i,j - 1].parent_i = i;
                    cellDetails[i,j - 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest, ref path);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i,j - 1] == false &&
                         isUnBlocked(grid, i, j - 1) == true)
                {
                    gNew = cellDetails[i,j].g + 1.0;
                    hNew = calculateHValue(i, j - 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i,j - 1].f == FLT_MAX ||
                            cellDetails[i,j - 1].f > fNew)
                    {
                        openList.Add(new Pair<double, Pair<int, int>>(fNew,
                                              new Pair<int,int>(i, j - 1)));

                        // Update the details of this cell 
                        cellDetails[i,j - 1].f = fNew;
                        cellDetails[i,j - 1].g = gNew;
                        cellDetails[i,j - 1].h = hNew;
                        cellDetails[i,j - 1].parent_i = i;
                        cellDetails[i,j - 1].parent_j = j;
                    }
                }
            }

            //----------- 5th Successor (North-East) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i - 1, j + 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i - 1, j + 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i - 1,j + 1].parent_i = i;
                    cellDetails[i - 1,j + 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest, ref path);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i - 1,j + 1] == false &&
                         isUnBlocked(grid, i - 1, j + 1) == true)
                {
                    gNew = cellDetails[i,j].g + 1.414;
                    hNew = calculateHValue(i - 1, j + 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i - 1,j + 1].f == FLT_MAX ||
                            cellDetails[i - 1,j + 1].f > fNew)
                    {
                        openList.Add(new Pair<double, Pair<int, int>>(fNew,
                                        new Pair<int,int>(i - 1, j + 1)));

                        // Update the details of this cell 
                        cellDetails[i - 1,j + 1].f = fNew;
                        cellDetails[i - 1,j + 1].g = gNew;
                        cellDetails[i - 1,j + 1].h = hNew;
                        cellDetails[i - 1,j + 1].parent_i = i;
                        cellDetails[i - 1,j + 1].parent_j = j;
                    }
                }
            }

            //----------- 6th Successor (North-West) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i - 1, j - 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i - 1, j - 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i - 1,j - 1].parent_i = i;
                    cellDetails[i - 1,j - 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest, ref path);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i - 1,j - 1] == false &&
                         isUnBlocked(grid, i - 1, j - 1) == true)
                {
                    gNew = cellDetails[i,j].g + 1.414;
                    hNew = calculateHValue(i - 1, j - 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i - 1,j - 1].f == FLT_MAX ||
                            cellDetails[i - 1,j - 1].f > fNew)
                    {
                        openList.Add(new Pair<double, Pair<int, int>>(fNew, new Pair<int,int>(i - 1, j - 1)));
                        // Update the details of this cell 
                        cellDetails[i - 1,j - 1].f = fNew;
                        cellDetails[i - 1,j - 1].g = gNew;
                        cellDetails[i - 1,j - 1].h = hNew;
                        cellDetails[i - 1,j - 1].parent_i = i;
                        cellDetails[i - 1,j - 1].parent_j = j;
                    }
                }
            }

            //----------- 7th Successor (South-East) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i + 1, j + 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i + 1, j + 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i + 1,j + 1].parent_i = i;
                    cellDetails[i + 1,j + 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest, ref path);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i + 1,j + 1] == false &&
                         isUnBlocked(grid, i + 1, j + 1) == true)
                {
                    gNew = cellDetails[i,j].g + 1.414;
                    hNew = calculateHValue(i + 1, j + 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i + 1,j + 1].f == FLT_MAX ||
                            cellDetails[i + 1,j + 1].f > fNew)
                    {
                        openList.Add(new Pair<double, Pair<int, int>>(fNew,
                                            new Pair<int,int>(i + 1, j + 1)));

                        // Update the details of this cell 
                        cellDetails[i + 1,j + 1].f = fNew;
                        cellDetails[i + 1,j + 1].g = gNew;
                        cellDetails[i + 1,j + 1].h = hNew;
                        cellDetails[i + 1,j + 1].parent_i = i;
                        cellDetails[i + 1,j + 1].parent_j = j;
                    }
                }
            }

            //----------- 8th Successor (South-West) ------------ 

            // Only process this cell if this is a valid one 
            if (isValid(i + 1, j - 1) == true)
            {
                // If the destination cell is the same as the 
                // current successor 
                if (isDestination(i + 1, j - 1, dest) == true)
                {
                    // Set the Parent of the destination cell 
                    cellDetails[i + 1,j - 1].parent_i = i;
                    cellDetails[i + 1,j - 1].parent_j = j;
                    //Debug.Log("The destination cell is found\n");
                    tracePath(cellDetails, dest, ref path);
                    foundDest = true;
                    return;
                }

                // If the successor is already on the closed 
                // list or if it is blocked, then ignore it. 
                // Else do the following 
                else if (closedList[i + 1,j - 1] == false &&
                         isUnBlocked(grid, i + 1, j - 1) == true)
                {
                    gNew = cellDetails[i,j].g + 1.414;
                    hNew = calculateHValue(i + 1, j - 1, dest);
                    fNew = gNew + hNew;

                    // If it isn’t on the open list, add it to 
                    // the open list. Make the current square 
                    // the parent of this square. Record the 
                    // f, g, and h costs of the square cell 
                    //                OR 
                    // If it is on the open list already, check 
                    // to see if this path to that square is better, 
                    // using 'f' cost as the measure. 
                    if (cellDetails[i + 1,j - 1].f == FLT_MAX ||
                            cellDetails[i + 1,j - 1].f > fNew)
                    {
                        openList.Add(new Pair<double, Pair<int, int>>(fNew,
                                            new Pair<int,int>(i + 1, j - 1)));

                        // Update the details of this cell 
                        cellDetails[i + 1,j - 1].f = fNew;
                        cellDetails[i + 1,j - 1].g = gNew;
                        cellDetails[i + 1,j - 1].h = hNew;
                        cellDetails[i + 1,j - 1].parent_i = i;
                        cellDetails[i + 1,j - 1].parent_j = j;
                    }
                }
            }
        } //END WHILE

        //=================================================================================//

        // When the destination cell is not found and the open 
        // list is empty, then we conclude that we failed to 
        // reach the destiantion cell. This may happen when the 
        // there is no way to destination cell (due to blockages) 
        if (foundDest == false) {
            Debug.Log("Failed to find the Destination Cell\n");
        }

        return;


    } //end a Star



} //end class
