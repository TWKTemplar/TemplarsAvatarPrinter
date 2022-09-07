using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[RequireComponent(typeof(MeshFilter))]
public class GhostMirror : MonoBehaviour
{
    //public void SetColorFloat(float colornum)
    //{   //0=red;.12=yellow;.25=lightGreen;.35=Green;.5=lightblue;.62=blue;.75=purple;.83=pink;1=red;
    //    Color = colornum;
    //}
    [Header("Settings")]
    [Tooltip("0=red;.12=yellow;.25=lightGreen;.35=Green;.5=lightblue;.62=blue;.75=purple;.83=pink;1=red;")]
    //[Range(0, 1)] public float Color;
    //[Range(0, 1)] public float yLevelGradiantMultiplier = 0;
    [Range(0, 1)] public float LocalDecimation = 1;
    [Header("Data")]
    [ReadOnlyInspector] public MeshFilter meshFilter;
    [ReadOnlyInspector] public Mesh mesh;
    public Vector3 RGBOfVertex()
    {
        Vector3 RGB = Vector3.zero;

        return RGB;
    } 

    public void GhostBounds(UnityEngine.Color ghostColor, UnityEngine.Color OutOfBoundsColor, UnityEngine.Color SelectedColor)
    {
        //Bounds Check
        List<Vector3> ListOfCornerPoints = new List<Vector3>();

        #region Create Bounding Grid
        Vector3 xyzBoundsSize = this.mesh.bounds.size;
        //Front
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f),transform));
        xyzBoundsSize.x *= -1;
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f), transform));
        xyzBoundsSize.y *= -1;
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f), transform));
        xyzBoundsSize.x *= -1;
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f), transform));
        //Back
        xyzBoundsSize.z *= -1;
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f), transform));
        xyzBoundsSize.x *= -1;
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f), transform));
        xyzBoundsSize.y *= -1;
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f), transform));
        xyzBoundsSize.x *= -1;
        ListOfCornerPoints.Add(PrinterUtils.LocalToWorldPos((Vector3)(this.mesh.bounds.center + xyzBoundsSize * 0.5f), transform));
        #endregion
        int NumberOfPointsOutOfBounds = 0;
        foreach (var point in ListOfCornerPoints)
        {

            if (Math.Abs(point.x) > 1 || Math.Abs(point.y) > 1 || Math.Abs(point.z) > 1)
            {
                Debug.LogWarning(this.name + " is outside bounds. OSC Printing will clamp to 1");
                Gizmos.color = UnityEngine.Color.red;
                Gizmos.DrawSphere(point, 0.05f);
                NumberOfPointsOutOfBounds++;
            }
            else
            {
                Gizmos.color = UnityEngine.Color.green;
                Gizmos.DrawSphere(point, 0.02f);
            }
        }
        Gizmos.color = ghostColor;
        if (NumberOfPointsOutOfBounds > 0) Gizmos.color = UnityEngine.Color.Lerp(Gizmos.color, OutOfBoundsColor,Mathf.Clamp(NumberOfPointsOutOfBounds* 0.5f,0,1f)) ;
        foreach (var selectedTransform in UnityEditor.Selection.transforms)
        {
            if (selectedTransform == transform)
            {
                if (NumberOfPointsOutOfBounds > 0) Gizmos.color = UnityEngine.Color.Lerp(Gizmos.color, SelectedColor, 0.5f);
                else Gizmos.color = SelectedColor;
            }
        }
        Gizmos.DrawWireMesh(this.mesh, this.transform.position, this.transform.rotation, this.transform.localScale);//Ghost Wireframe
    }
}
