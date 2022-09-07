using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GhostMirror)), CanEditMultipleObjects]
public class GhostMirrorEditor : Editor
{
    public override void OnInspectorGUI()
    {

        GhostMirror ghostMirror = (GhostMirror)target;
        
        //This UI works only for single object editing which could cause confusion to some users, it will remain here until I figure out multi object button calls
        //GUILayout.BeginHorizontal("box");
        //if (GUILayout.Button("Random")) { ghostMirror.SetColorFloat(Random.Range(0f,1f)); }
        //GUILayout.EndHorizontal();
        //GUILayout.BeginHorizontal("box");
        //if (GUILayout.Button("0"))  { ghostMirror.SetColorFloat(0.0f); }
        //if (GUILayout.Button(".1")) { ghostMirror.SetColorFloat(0.1f); }
        //if (GUILayout.Button(".2")) { ghostMirror.SetColorFloat(0.2f); }
        //if (GUILayout.Button(".3")) { ghostMirror.SetColorFloat(0.3f); }
        //if (GUILayout.Button(".4")) { ghostMirror.SetColorFloat(0.4f); }
        //if (GUILayout.Button(".5")) { ghostMirror.SetColorFloat(0.5f); }
        //if (GUILayout.Button(".6")) { ghostMirror.SetColorFloat(0.6f); }
        //if (GUILayout.Button(".7")) { ghostMirror.SetColorFloat(0.7f); }
        //if (GUILayout.Button(".8")) { ghostMirror.SetColorFloat(0.8f); }
        //if (GUILayout.Button(".9")) { ghostMirror.SetColorFloat(0.9f); }
        //if (GUILayout.Button("1"))  { ghostMirror.SetColorFloat(1.0f); }
        //GUILayout.EndHorizontal();
        Texture ColorBanner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/MyStuff/3D Printer/Resources/colors.png", typeof(Texture));
        GUILayout.Label(ColorBanner, GUILayout.Width(10), GUILayout.MaxWidth(600), GUILayout.Height(50), GUILayout.MaxHeight(50));
        DrawDefaultInspector();

    }
}
