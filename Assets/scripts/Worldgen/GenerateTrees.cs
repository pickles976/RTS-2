using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTrees : MonoBehaviour
{

    public float xBounds = 160.0f;
    public float zBounds = 160.0f;

    public float minHeight;
    public float maxHeight;

    //0.435 seems to give us a nice forest w/ cool clearings at smaller scales (used for 160f x 160f bounds)
    public float probability = 0.75f;

    public GameObject prefab;

    //0.0111 gives us nice dense forests with clearings (used for 160f x 160f bounds)
    //0.0211 gives us too much clearing and too much impassability
    //0.0333 creates a less dense, more scattered forest with lots of traversable space
    public float scaler = 0.11f;

    public float seed;

    public bool autoUpdate;

    List<GameObject> tree_list;

    //populate with items
    public void generateTrees()
    {

        removeAllTrees();

        tree_list = new List<GameObject>();

        GameObject temp;

        Random.seed = (int)seed; //makes our random numbers repeatable

        RaycastHit hit;

        float offset = 1345.67f;

        offset += seed;

        //loop through the desired grid and populate with trees

        float z = -zBounds;

        while(z < zBounds) //z
        {
            float x = -xBounds;

            while(x < xBounds) //x
            {
                float sample = Mathf.PerlinNoise((x * scaler) + offset, (z * scaler) + offset); //get Perlin number
                float gauss = 8f * Random.Range(0f, 1f);

                if (sample < probability) //spawn a tree with perlin
                {
                    Vector3 origin = new Vector3(x, 100.0f, z);

                    if (Physics.Raycast(origin,-Vector3.up,out hit,Mathf.Infinity,(1<<8)))
                    {
                        if (hit.point.y > minHeight && hit.point.y < maxHeight) {
                            temp = Instantiate(prefab, new Vector3(hit.point.x + Random.Range(-3.33f, 3.33f),hit.point.y -0.5f, hit.point.z + Random.Range(-3.33f, 3.33f)), new Quaternion());
                            tree_list.Add(temp);
                            //Instantiate(prefab, new Vector3(x + 0.5f, -0.5f, z + 0.5f), new Quaternion());
                        }
                    }
                }
                else if (gauss < probability)
                {
                    Vector3 origin = new Vector3(x, 100.0f, z);

                    if (Physics.Raycast(origin, -Vector3.up, out hit, Mathf.Infinity, (1 << 8)))
                    {
                        if (hit.point.y > 4.0f && hit.point.y < 8f)
                        {
                            temp = Instantiate(prefab, new Vector3(hit.point.x + Random.Range(-3.33f, 3.33f), hit.point.y - 0.5f, hit.point.z + Random.Range(-3.33f, 3.33f)), new Quaternion());
                            tree_list.Add(temp);
                            //Instantiate(prefab, new Vector3(x + 0.5f, -0.5f, z + 0.5f), new Quaternion());
                        }
                    }
                }
                
                x += 10.0f;
            }
            z += 10.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(this);
    }

    //delete all of the trees
    void removeAllTrees()
    {
        if (tree_list != null) {
            foreach (GameObject tree in tree_list)
            {
                DestroyImmediate(tree);
            }
        }
    }
}
