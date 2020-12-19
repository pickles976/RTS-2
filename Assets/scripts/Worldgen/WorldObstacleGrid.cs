using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObstacleGrid : MonoBehaviour
{

    generate_obstacle_grid obs_grid_generator;

    float cellSize;
    int xLength;
    int zLength;
    int xOffset;
    int zOffset;

    int[,] obstacle_grid;

    Renderer textureRenderer;

    GameObject terrain;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GameObject.Find("Terrain");

        obs_grid_generator = new generate_obstacle_grid();

        //assume the mesh is centered at 0,0
        //set the offset to the bottom-left corner, since the obstacle grid scans from left to right, bottom to top
        xOffset = -1 * (int)terrain.GetComponent<MeshCollider>().bounds.extents.x;
        zOffset = -1 * (int)terrain.GetComponent<MeshCollider>().bounds.extents.z;

        xLength = (int)terrain.GetComponent<MeshCollider>().bounds.size.x;
        zLength = (int)terrain.GetComponent<MeshCollider>().bounds.size.z;

        cellSize = 5.0f;

        obstacle_grid = obs_grid_generator.Generate_grid(cellSize, xLength, zLength, xOffset, zOffset);

        showBlockedSubCells();
    }

    public void showBlockedSubCells()
    {

        float[,] textureMap = new float[xLength, zLength];

        //convert int to float
        for (int i = 0; i < xLength; i++)
        {
            for (int j = 0; j < zLength; j++)
            {
                textureMap[i, j] = obstacle_grid[i, j];
            }
        }

        textureRenderer = GetComponent<Renderer>();
        textureRenderer.sharedMaterial.mainTexture = TextureGenerator.TextureFromHeightMap(textureMap);
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
    }
    
    //1 is blocked
    //0 is unblocked
    public int[,] getBlockedSuperCells(){
        return new int[1,1];
    }



}
