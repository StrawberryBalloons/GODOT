using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DrawHeightMap))]
public class MapgenEdit : Editor
{

    public override void OnInspectorGUI()
    {
        DrawHeightMap mGen = (DrawHeightMap)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            mGen.decideTexture();
        }
    }
}