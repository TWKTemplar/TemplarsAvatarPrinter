using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.UIElements;

[CustomEditor(typeof(ScreenshotManager))]
public class ScreenshotManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ScreenshotManager screenshotManager = (ScreenshotManager)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Show in explorer"))
        {
            screenshotManager.ShowExplorer();
        }

        if (GUILayout.Button("Take Screen Shot"))
        {
            screenshotManager.TakeScreenShot();
        }
    }
}
