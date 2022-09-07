using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.UIElements;

[CustomEditor(typeof(PrinterCPU))]
public class PrinterCPUEditor : Editor
{
    public override void OnInspectorGUI()
    {
       PrinterCPU printerCPU = (PrinterCPU)target;
        GUILayout.Space(10);

        GUILayout.Label("Time remaining : " + printerCPU.TimeRemaining);
        GUILayout.Space(10);

        DrawDefaultInspector();
       if (GUILayout.Button("Do 1 Manual Cycle"))
       {
           printerCPU.StartParticlePlacement();
       }
    }
    
}
