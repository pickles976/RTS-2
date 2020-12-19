using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tree_size_rand : MonoBehaviour
{

    public float range = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        float scale = Random.Range(-range, range);

        transform.localScale += new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(this);
    }
}
