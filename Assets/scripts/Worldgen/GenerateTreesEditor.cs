using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateTrees))]
public class GenerateTreesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GenerateTrees treeGen = (GenerateTrees)target;

        if (DrawDefaultInspector())
        {
            if (treeGen.autoUpdate)
            {
                treeGen.generateTrees();
            }
        }

    }
}
