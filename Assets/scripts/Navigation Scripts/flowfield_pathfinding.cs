using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowfield_pathfinding : MonoBehaviour
{

    public custom_grid superGrid; // cellSize 10
    public custom_grid subGrid; // cellSize 1, default size 10x10
    public int grid_ratio = 10; //ratio of subgrid cells per supergrid

    public DijkstraGrid djgrid; //for debug drawing purposes only

    RaycastHit hit; //Raycasthit
    Vector3 destination;

    Stack<Pair<int, int>> Path;

    int current_i; //current supergrid coordinates
    int current_j;

    List<Pair<int,int>> aStarBlocked; //updated when an astar cell becomes blocked
    int buffer; //this is used to expand the bounds of our Astar search grid whenever the path is blocked

    public bool show_dijkstra_debug = false; //draws the ff and dijkstra grids
    public bool show_ff_debug = true;
    public bool show_blocked_debug = false;
    public bool show_astar_debug = false;

    // Start is called before the first frame update
    void Awake()
    {
        subGrid = new custom_grid();
        subGrid.cellSize = 5.0f;

        superGrid = new custom_grid();
        superGrid.cellSize = 50.0f;

        destination = transform.position;

        Path = new Stack<Pair<int, int>>(); //initialize empty path

        djgrid = new DijkstraGrid(); //dijkstra grid is created during flowfield generation, it's used to determine if a cell is blocked or not

    }

    // Update is called once per frame
    void Update()
    {
        if ((Path.Count > 0)) //is there still another item in the stack?
        {
            //Debug.Log("current_i = " + current_i + " current_j: " + current_j);

            //Are we at the next item in the stack?
            if ((superGrid.worldToCell(transform.position).Item1 == Path.Peek().first) && (superGrid.worldToCell(transform.position).Item2 == Path.Peek().second))
            {
                //dequeue Astar cell
                Path.Pop();
                current_i = superGrid.worldToCell(transform.position).Item1;
                current_j = superGrid.worldToCell(transform.position).Item2;

                    int next_i;
                    int next_j;

                    if (Path.Count > 0)
                    {
                        next_i = Path.Peek().first;
                        next_j = Path.Peek().second;
                    }
                    else
                    {
                        next_i = superGrid.worldToCell(transform.position).Item1;
                        next_j = superGrid.worldToCell(transform.position).Item2;
                    }
                    getNextFlowfield(current_i, current_j, next_i, next_j);
                return;

            }
            else //if not at next grid spot
            {
                //if we are NOT still on track
                if ((current_i != superGrid.worldToCell(transform.position).Item1) || (current_j != superGrid.worldToCell(transform.position).Item2))
                {
                    //Debug.Log("crossed into a non-stack grid");
                    current_i = superGrid.worldToCell(transform.position).Item1;
                    current_j = superGrid.worldToCell(transform.position).Item2;
                    Path.Push(new Pair<int,int>(current_i, current_j)); //add our current grid space onto the stack, we will calculate a flowfield next frame
                }
            }

        }
        else //if the stack is empty
        { 
            if((transform.position - destination).magnitude > 5.0f) //if we are not at the destination
            {
                int current_i = superGrid.worldToCell(transform.position).Item1;
                int current_j = superGrid.worldToCell(transform.position).Item2;
                int next_i = superGrid.worldToCell(transform.position).Item1;
                int next_j = superGrid.worldToCell(transform.position).Item2;

                getNextFlowfield(current_i, current_j, next_i, next_j); //generate a flowfield for our current cell to the destination
                return;
            }
        }//while stack not empty  
    }

    public void setDestination(Vector3 dest) //starts pathfinding from scratch (clears list of blocked supergrids)
    {
        destination = dest;

        buffer = 0;
        aStarBlocked = new List<Pair<int, int>>();

        getAstarPath(ref Path);
    }


//======================================================================================================//

    //generates stack for our flowfields
    void getAstarPath(ref Stack<Pair<int,int>> path)
    {
                //Debug.Log("Starting cells: " + superGrid.worldToCell(transform.position));

                //extract position data from tuples
                int i_start = superGrid.worldToCell(transform.position).Item1;
                int j_start = superGrid.worldToCell(transform.position).Item2;
                int i_end = superGrid.worldToCell(destination).Item1;
                int j_end = superGrid.worldToCell(destination).Item2;
                var astar = new Astar_search();

        //we need to perform a coordinate transform on our starting and destination 
        //points and then re-transform them to get global coordinates

        //WHAT TO IMPLEMENT::
        //ORIGINAL GRID POSITIONS
        /* // Suppose we originally draw a path from [1,1] to [3,2] on the current grid
         *[0,3][1,3][2,3][3,3]
         *[0,2][1,2][2,2][3,2]
         *[0,1][1,1][2,1][3,1]
         *[0,0][1,0][2,0][3,0]
         */

        //The first step is to figure out how to index the array. For this example [1,1] should be [0,0] 
        //and [3,2] should be [2,1]. That way we can calculate an Astar path with normalized indices (start point
        //equal to [0,0])

        //However, suppose we go from [3,0] to [1,3]
        //In this case [1,0] will become [0,0] and [1,3] will become [0,3] and [3,0] will become [2,0]

        //WHAT THIS FUNCTION DOES
        //let's re-use our example of [1,1] to [3,2] again
        //if we create an array with width equal to (horizontal displacement x 2) + 1, and height equal to 
        //(vertical displacement x 2) + 1, then we will have an array where our start position is at the center

        /* X = [1,1] and Y = [3,2]
         * [0][0][0][0][Y]
         * [0][0][X][0][0]
         * [0][0][0][0][0]
         */

        //notice how 75% of this grid is space that will not be searched, 
        //this is pretty inefficient. Instead of assuming a width of 
        //displacement x2 - 1, we will do a width of displacement + 1 + a buffer (more about the buffer later)
        //our new grid looks like this:

        /* X = [1,1] and Y = [3,2]
        * [0][0][Y]
        * [X][0][0]
        * 
        * height = (2 - 1) + 1 = 2
        * width = (3 - 1) + 1 = 3
        * origin is at bottom-left, destination is at top-right
        */

        //this works well for an example where we go north-east, north, or east. However different displacements
        //require different grids

        /* X = [3,0] and Y = [1,3]
         * [Y][0][0]
         * [0][0][0]
         * [0][0][0]
         * [0][0][X]
         */

        //now the start point is in the bottom-right, and our end position is in the top-left
        //these positions are calculated based on which quadrant we start and end in, as well as the buffer size of the A*
        //this coordinate transform is what allows us to perform an A* in all directions in both positive and negative
        //world space


        int ROW = (Mathf.Abs(i_end - i_start)) + (2 * buffer) + 1; //figure out the size of the array to create
        int COL = (Mathf.Abs(j_end - j_start)) + (2 * buffer) + 1;

                int i_local_end = 0;
                int j_local_end = 0;
                int i_local_start = 0;
                int j_local_start = 0;
                int i_offset = 0;
                int j_offset = 0;

        //CALCULATE START AND END INDICES RELATIVE TO INTEGER ARRAY
        //[ ][E]
        //[S][ ]
        if (i_end >= i_start && j_end >= j_start)
        {
            i_local_start = buffer;
            j_local_start = buffer;
            i_local_end = ROW - 1 - buffer;
            j_local_end = COL - 1 - buffer;
        }
        //[S][ ]
        //[ ][E]
        else if (i_end > i_start && j_end < j_start)
        {
            i_local_start = buffer;
            j_local_start = COL - 1 - buffer;
            j_offset = COL - 1 - (2*buffer);
            i_local_end = ROW - 1 - buffer;
            j_local_end = buffer;
        }
        //[ ][S]
        //[E][ ]
        else if (i_end <= i_start && j_end <= j_start)
        {
            i_local_start = ROW - 1 - buffer;
            j_local_start = COL - 1 - buffer;
            i_offset = ROW - 1 - (2 * buffer);
            j_offset = COL - 1 - (2 * buffer);
            i_local_end = buffer;
            j_local_end = buffer;
        }
        //[E][ ]
        //[ ][S]
        else if (i_end < i_start && j_end > j_start)
        {
            i_local_start = ROW - 1 - buffer;
            j_local_start = buffer;
            i_offset = ROW - 1 - (2 * buffer);
            i_local_end = buffer;
            j_local_end = COL - 1 - buffer;
        }

        //Debug.Log("Ending cells: " + superGrid.worldToCell(destination));
        //Debug.Log("Buffer size: " + buffer);
        //Debug.Log("i_local_start: " + i_local_start + " j_local_start: " + j_local_start);
        //Debug.Log("i_local_end: " + i_local_end + " j_local_end: " + j_local_end);


        //This part updates our list of blocked cells before we calculate a new path
        //this is similar to a D* search

                var temp_grid = new int[ROW, COL];

                for (int i = 0; i < ROW; i++) //create an array of unblocked cells (1 open, 0 blocked)
                {
                    for (int j = 0; j < COL; j++)
                    {
                        temp_grid[i, j] = 1;
                    }
                }

                foreach (Pair<int, int> blockedCell in aStarBlocked) //loop through the list of blocked cells and transform to local coordinates, then apply
                {
                    //apply the offset
                    int i_index = ((blockedCell.first) - i_start + i_offset + buffer);
                    int j_index = ((blockedCell.second) - j_start + j_offset + buffer);
                    //Debug.Log("blocked_i_index: " + i_index + " blocked_j_index: " + j_index);

                    if ((i_index < ROW && j_index < COL) && (i_index >= 0 && j_index >= 0))
                    {
                        temp_grid[i_index, j_index] = 0;
                    }

                    //if the destination is blocked, get the next best thing
                    if((i_index == i_local_end) && (j_index == j_local_end)){

                    //convert our blocked grid to a new format so we can find the closest unblocked neighbor
                    int[,] new_grid = new int[ROW, COL];

                    for (int i = 0; i < ROW; i++) //create an array of unblocked cells (Int32.MaxValue = BLOCKED)
                    {
                        for (int j = 0; j < COL; j++)
                        {
                            if(temp_grid[i, j] == 0)
                            {
                                new_grid[i, j] = Int32.MaxValue; //our "find_nearest_unblocked" function only sees Int32.MaxValue as a blocked space
                            }
                        }
                    }

                        //get a new pair of destination cells
                        generate_obstacle_grid obstacle_helper = new generate_obstacle_grid(); //create this to use its' utility function
                        Pair<int,int> newDestPair = obstacle_helper.find_nearest_unblocked(new_grid, i_local_end, j_local_end, ROW, COL);
                        i_local_end = newDestPair.first;
                        j_local_end = newDestPair.second; 
                        //Debug.Log("Rowsize = " + ROW + " Colsize = " + COL);
                        //Debug.Log("new_dest_i: " + i_local_end + " new_dest_j: " + j_local_end);
                    }
                }

                //finally calculate the new A star search
                astar.aStarSearch(temp_grid, new Pair<int, int>(i_local_start, j_local_start), new Pair<int, int>(i_local_end, j_local_end), ROW, COL,ref path); //print A* stuff

                Stack<Pair<int, int>> temp_path = new Stack<Pair<int, int>>();

                //restore global coordinates

                //create temporary Stack that holds our restored path values
                while (!(path.Count == 0)) //while stack not empty
                {
                    Pair<int, int> p = path.Pop(); //get item off top of stack
                    //re-apply the offset
                    p.first = ((p.first) + i_start - i_offset - buffer);
                    p.second = ((p.second) + j_start - j_offset - buffer);
                    temp_path.Push(p);
                }

                //restore Stack
                while (!(temp_path.Count == 0)) //while stack not empty
                {
                    path.Push(temp_path.Pop());
                }

    }//end getAStarPath

    //=====================================================================================================================//
    
    //Given a start and an end supergrid, we create compound flowfield between both grids and any adjacent tiles (if they are diagonal)
    public void getNextFlowfield(int curr_i,int curr_j, int nex_i, int nex_j)
    {

        //GetComponent<unit_behavior>().ff_table.deleteFF(gameObject.GetInstanceID()); //unlink the previous flowfield from this object's id

        int current_i = curr_i;
        int current_j = curr_j;
        int next_i = nex_i;
        int next_j = nex_j;

        int row_length = 0;
        int col_length = 0;
        int xOff = 0; //offset for calculating global
        int zOff = 0; //grid indices
        int destRow = 0; //calculate local grid 
        int destCol = 0; //indices
        int type = 0; //tells us which direction we are generating our flowfield in

        //this way we can figure out what cells are blocked when plotting a route
        generate_obstacle_grid obs = new generate_obstacle_grid();

        //row_length, col_length, xOff,zOff, and destrow and col depend
        //on which direction the next cell is in

        //[SE]
        if (current_i == next_i && current_j == next_j)
        {
            row_length = grid_ratio;
            col_length = grid_ratio;
            xOff = grid_ratio * (current_i);
            zOff = grid_ratio * (current_j);
            destCol = subGrid.worldToCell(destination).Item1 % grid_ratio;
            destRow = subGrid.worldToCell(destination).Item2 % grid_ratio;

            if (destCol < 0) //deal with negatives
            {
                destCol = grid_ratio + destCol;
            }

            if (destRow < 0)
            {
                destRow = grid_ratio + destRow;
            }

            type = 0;
        }
        //[S][E]
        else if (current_i < next_i && current_j == next_j)
        {
            row_length = 2 * grid_ratio; //for dynamic sizing, use 10 * Math.abs(current_i - next_i)
            col_length = grid_ratio;
            xOff = grid_ratio * (current_i);
            zOff = grid_ratio * (current_j);
            destCol = grid_ratio / 2;  //these are fixed destinations, we need to dynamically
            destRow = (2 * grid_ratio) - 1; //pick destinations based on which cells are blocked or not
            type = 0;
        }
        //[ ][E]
        //[S][ ] 2.
        else if (current_i < next_i && current_j < next_j)
        {
            row_length = 2 * grid_ratio;
            col_length = 2 * grid_ratio;
            xOff = grid_ratio * (current_i);
            zOff = grid_ratio * (current_j);
            destCol = (2 * grid_ratio) - 1;
            destRow = (2 * grid_ratio) - 1;
            type = 0;
        }
        //[E]
        //[S] 3.
        else if (current_i == next_i && current_j < next_j)
        {
            row_length = grid_ratio;
            col_length = 2*grid_ratio;
            xOff = grid_ratio * (current_i);
            zOff = grid_ratio * (current_j);
            destCol = (2*grid_ratio) - 1;
            destRow = grid_ratio / 2;
            type = 0;
        }
        //[E][ ]
        //[ ][S] 4.
        else if (current_i > next_i && current_j < next_j)
        {
            row_length = 2*grid_ratio;
            col_length = 2*grid_ratio;
            xOff = grid_ratio * (current_i - 1);
            zOff = grid_ratio * (current_j);
            destCol = (2*grid_ratio) - 1;
            destRow = 0;
            type = 1;
        }
        //[E][S] 5.
        else if (current_i > next_i && current_j == next_j)
        {
            row_length = 2*grid_ratio;
            col_length = grid_ratio;
            xOff = grid_ratio * (current_i - 1);
            zOff = grid_ratio * (current_j);
            destCol = grid_ratio / 2;
            destRow = 0;
            type = 1;
        }
        //[ ][S]
        //[E][ ] 6.
        else if (current_i > next_i && current_j > next_j)
        {
            row_length = 2*grid_ratio;
            col_length = 2*grid_ratio;
            xOff = grid_ratio * (current_i - 1);
            zOff = grid_ratio * (current_j - 1);
            destCol = 0;
            destRow = 0;
            type = 2;
        }
        //[S]
        //[E] 7.
        else if (current_i == next_i && current_j > next_j)
        {
            row_length = grid_ratio;
            col_length = 2 * grid_ratio;
            xOff = grid_ratio * (current_i);
            zOff = grid_ratio * (current_j - 1);
            destCol = 0;
            destRow = grid_ratio/2;
            type = 3;
        }
        //[S][ ]
        //[ ][E]
        else if (current_i < next_i && current_j > next_j)
        {
            row_length = 2*grid_ratio;
            col_length = 2*grid_ratio;
            xOff = grid_ratio * (current_i);
            zOff = grid_ratio * (current_j - 1);
            destCol = 0;
            destRow = (2*grid_ratio)-1;
            type = 3;
        }

        //find the obstacles
        int[,] obstacle_grid = obs.Generate_grid(subGrid.cellSize, row_length, col_length, xOff, zOff);

        if (Path.Count == 0) //if we are at the final supergrid location
        {
            destRow = subGrid.worldToCell(destination).Item1 % grid_ratio; //subgrid cells per supergrid
            destCol = subGrid.worldToCell(destination).Item2 % grid_ratio;

            //needs to be done for negative indices
            if (destRow < 0)
            {
                destRow = grid_ratio + destRow;
            }

            if (destCol < 0)
            {
                destCol = grid_ratio + destCol;
            }

            //allows us to flowfield in all 4 quadrants
            switch (type)
            {
                case 0:
                    break;

                case 1:
                    destRow = destRow + grid_ratio;
                    break;

                case 2:
                    destRow = destRow + grid_ratio;
                    destCol = destCol + grid_ratio;
                    break;

                case 3:
                    destCol = destCol + grid_ratio;
                    break;

            }
        }

        //get destination (find nearest unblocked cell)
        Pair<int, int> p = obs.find_nearest_unblocked(obstacle_grid, destRow, destCol, row_length, col_length);
        destRow = p.first;
        destCol = p.second;


        
        //make sure that the destination isn't entirely blocked
        //if it isn't, then we say that the cell is blocked
        switch (type)
        {
            case 0:
                if (destRow < grid_ratio && destCol < grid_ratio)
                {
                    if (next_i != current_i || next_j != current_j) //make sure we are not in the destination cell
                    {
                        aStarBlocked.Add(new Pair<int, int>(next_i, next_j)); //
                        buffer++; //expand our search grid
                        getAstarPath(ref Path); //recalculate path
                        return;
                    }
                }
                break;
                
                
            case 1:
                //Debug.Log("Destrow is: " + destRow);
                //Debug.Log("Destcol is: " + destCol);
                if (destCol < grid_ratio && destRow >= grid_ratio)
                {
                    aStarBlocked.Add(new Pair<int, int>(next_i, next_j)); //
                    buffer++; //expand our search grid
                    getAstarPath(ref Path); //recalculate path
                    return;
                }
                break;
                

                /*
            case 2:
                if (destRow >= grid_ratio && destCol >= grid_ratio)
                {
                    aStarBlocked.Add(new Pair<int, int>(next_i, next_j)); //
                    buffer++; //expand our search grid
                    getAstarPath(ref Path); //recalculate path
                    return;
                }
                break;
                */

                
            case 3:
                if (destRow < grid_ratio && destCol >= grid_ratio)
                {
                    aStarBlocked.Add(new Pair<int, int>(next_i, next_j)); //
                    buffer++; //expand our search grid
                    getAstarPath(ref Path); //recalculate path
                    return;
                }
                break;
                

            default: break;
                
        }

    


        //GENERATE FLOWFIELD WITH GIVEN PARAMETERS
        Djikstra dijk = new Djikstra();
        //Debug.Log("Type = " + type);
        //Debug.Log("row_length: " + row_length + " col_length: " + col_length);
        //Debug.Log("destCol: " + destCol + " destRow: " + destRow);
        int[,] dijkstra = dijk.generate_djikstra_grid(row_length, col_length, obstacle_grid, new Pair<int, int>(destCol, destRow));

        //these values contextualize our Dijkstra grid for future use
        djgrid.grid = dijkstra;
        djgrid.row_length = row_length;
        djgrid.col_length = col_length;
        djgrid.type = type;
        djgrid.xOff = xOff;
        djgrid.zOff = zOff;

        //if there is no path, mark this astar cell as blocked
        if(djgrid.getValuefromObject(gameObject) == -1)
        {
                //Debug.Log("next_i: " + next_i + " next_j: " + next_j);
                aStarBlocked.Add(new Pair<int, int>(next_i, next_j)); //
                buffer++; //expand our search grid
                getAstarPath(ref Path); //recalculate path
                return;
        }
        else if (djgrid.getValuefromObject(gameObject) == 0) //if we have reached an objective, but this function is still active for some reason, reset our pathfinding
        {
            setDestination(destination);
            return;
        }

        Generate_Flowfield field_obj = new Generate_Flowfield();
        Vector3[,] field = field_obj.generate_flowfield(row_length, col_length, dijkstra, xOff, zOff, subGrid.cellSize);

        //these values also contextualize our flowfield for future use 
        Flowfield ff = new Flowfield();
        ff.flowfield = field;
        ff.row_length = row_length;
        ff.col_length = col_length;
        ff.type = type;
        ff.xOff = xOff;
        ff.zOff = zOff;

            GetComponent<unit_behavior>().ff_table.addFF(gameObject.GetInstanceID(), ff); //add flowfield to dictionary and link by ID

    }//end function


    //draw a dijkstra grid
    private void OnDrawGizmos()
    {

        //show blocked cells
        if (show_blocked_debug == true)
        {
            if (aStarBlocked != null)
            {
                foreach (Pair<int,int> cell in aStarBlocked)
                {
                    float xPos = cell.first * grid_ratio * subGrid.cellSize + (0.5f * grid_ratio * subGrid.cellSize);
                    float zPos = cell.second * grid_ratio * subGrid.cellSize + (0.5f * grid_ratio * subGrid.cellSize);
                    UnityEditor.Handles.Label(new Vector3(xPos, 1.0f, zPos), "BLOCKED");
                }
            }
        }

        //show blocked cells
        if (show_astar_debug == true)
        {
            if (Path != null)
            {
                foreach (Pair<int, int> cell in Path)
                {
                    float xPos = cell.first * grid_ratio * subGrid.cellSize + (0.5f * grid_ratio * subGrid.cellSize);
                    float zPos = cell.second * grid_ratio * subGrid.cellSize + (0.5f * grid_ratio * subGrid.cellSize);
                    UnityEditor.Handles.Label(new Vector3(xPos, 1.0f, zPos), "X: " + cell.first + " Z: " + cell.second);
                }
            }
        }

        //show Dijkstra grid
        if (show_dijkstra_debug)
        {
            for (int j = 0; j < djgrid.col_length; j++)
            {
                for (int i = 0; i < djgrid.row_length; i++)
                {
                    System.Tuple<int, int> offset = superGrid.worldToCell(transform.position);
                    float xpos = (float)(i + djgrid.xOff + 0.5) * subGrid.cellSize;
                    float zpos = (float)(j + djgrid.zOff + 0.5) * subGrid.cellSize;

                    string val;
                    if (djgrid.grid[i, j] == System.Int32.MaxValue)
                    {
                        val = "MAX";
                    }
                    else
                    {
                        val = djgrid.grid[i, j].ToString();
                    }

                    UnityEditor.Handles.Label(new Vector3(xpos, 1.0f, zpos), val);
                }

            }
        }
    }//end draw dijkstra grid

    //if we collide with an obstacle then it means the world has been updated. Calculate a new path accordingly
    private void OnCollisionEnter(Collision collision)
    {

        //Debug.Log("Collision!");
        /*
        if (collision.gameObject.layer == 10) //check if we have touched an obstacle collider
        {
            //Debug.Log("Destination reset!");
            setDestination(destination);
        }
        */
    }

}
