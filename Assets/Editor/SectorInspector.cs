using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SectorPlacesGenerator))]
public class SectorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SectorPlacesGenerator sectorGenerator = (SectorPlacesGenerator)target;
        if (GUILayout.Button("Generate place for container"))
        {
            sectorGenerator.GenerateSpaces();
        }
    }
}
