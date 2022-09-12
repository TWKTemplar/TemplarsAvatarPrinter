using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// The Ghost manager is focused on gathering the original lists of verts from the mesh filter gameobjects.
    /// void SetUpPrintObjects() is a method that removes mesh renderers and replaces them with a GhostMirror script. 
    /// The added GhostMirror script is what calculates the mesh outline and the mesh's bounding box. This bounding box colors red when the 
    /// item is outside of -1 to 1 bounding box for printing.
    /// 
    /// void MergeByDistance() is responcable for creating a tupple lookup table to remove points that are too close togeather. 
    /// This allows for a 50% overall decreese in time for all prints as most meshes set to 'Normals:Import' rather than none/calculated which have double the desired verts
    /// 
    /// 
    /// </summary>



    [Header("Build Options (reduce build time here!)")]
    [Tooltip("Percent of verts to keep, applied before Resolution calculation")][Range(0f, 1)] public float Decimation = 1;
    [Tooltip("Less resolution => Larger merge by distance range")][Range(10,60)]public int Resolution = 30;//The higher this number the closer points would be allowed to not be merged into 1 point
    [Tooltip("Verts gathered in active gameobjects")][ReadOnlyInspector] public int TotalVertsInMeshArray;
    [Tooltip("Remaining verts after Decimation and Resolution")][ReadOnlyInspector] public int TotalVertsInMeshArrayFinal;
    [Tooltip("Toggles the use of SkipBlackVertexColorsCutOff when calculating what verts to print")]
    public bool SkipBlackVertexColors = false;
    [Tooltip("Any HSV colors with a value lower than SkipBlackVertexColorsCutOff will be omitted")]
    [Range(0f, 1f)] public float SkipBlackVertexColorsCutOff = 0.1f;

    [Header("Options (Unity preview focused)")]
    public Color GhostColor;
    public Color SelectedColor;
    public Color OutOfBoundsColor;
    [Tooltip("The dynamic list of active child gameobjects with a mesh filter")][ReadOnlyInspector] public List<GhostMirror> PrintItems;
    [Tooltip("The dynamic list of active child gameobjects with a mesh filter count")] [ReadOnlyInspector] public int itemLength = 0;
    [Header("Data")]
    [ReadOnlyInspector] public List<PrinterMem.PrinterCommand> printerCommands = new List<PrinterMem.PrinterCommand>();//Is this is a dynamicly updating list of all verts's color, position and ID
    [Header("Required Ref")]
    public PrinterMem printerMem;
    public void Awake()
    {   
        //Hide Mesh Renderers on start
        foreach (var item in PrintItems)
        {
            var meshrend = item.meshFilter.GetComponent<MeshRenderer>();
            if (meshrend != null) meshrend.enabled = false;
        }
    }
    public void SetUpPrintObjects()
    {
        PrintItems.Clear();
        MeshFilter[] meshFilterarray = transform.GetComponentsInChildren<MeshFilter>();
        if (meshFilterarray.Length == 0) return;
        foreach (MeshFilter meshFilter in meshFilterarray)
        {
            GhostMirror ghostMirror = null;
            ghostMirror = meshFilter.GetComponent<GhostMirror>();
            if (ghostMirror == null)
            {
                ghostMirror = meshFilter.gameObject.AddComponent<GhostMirror>();
                ghostMirror.meshFilter = meshFilter;
                ghostMirror.mesh = meshFilter.sharedMesh;
            }
            else
            {
                ghostMirror = meshFilter.gameObject.GetComponent<GhostMirror>();
            }

            if(ghostMirror.gameObject.activeInHierarchy) PrintItems.Add(ghostMirror);
        }
        RecacheMeshFilters();
    }
    public void RecacheMeshFilters()
    {
        GhostMirror[] ghostMirrorArray = transform.GetComponentsInChildren<GhostMirror>();
        foreach (GhostMirror ghostMirror in ghostMirrorArray)
        {
            ghostMirror.mesh = ghostMirror.meshFilter.sharedMesh;
        }
    }
    [ContextMenu("CalculatePrinterCommands")]
    public void CalculatePrinterCommands()
    {
        printerCommands.Clear();
        //Initalize Tuple, used to reduce the number of unneeded points while printing
        var sceenpoints = new HashSet<Tuple<int, int, int>>();
        foreach (var printItem in PrintItems)
        {
            for (int i = 0; i < printItem.mesh.vertices.Length; i++)
            {
                var point = printItem.mesh.vertices[i];
                
                //Decimation calculation
                bool ran1 = UnityEngine.Random.value > Decimation * printItem.LocalDecimation;
                if (ran1) continue;

                if (SkipBlackVertexColors)
                {
                    // vertexColor
                    var vertexColorTemp = printItem.mesh.colors[i];
                    Vector3 vertexColorTemp2 = Vector3.one;
                    Color.RGBToHSV(vertexColorTemp, out vertexColorTemp2.x, out vertexColorTemp2.y, out vertexColorTemp2.z);//HSV = xyz, h=x, s=y, v=z
                    if(vertexColorTemp2.z < 0.1f)//Value is lower than 0.5f
                    {
                        continue;
                    }

                }


                //Merge by distance calculation
                Vector3 tuppos = PrinterUtils.LocalToWorldPos(point, printItem.transform);
                var tup = new Tuple<int, int, int>(Mathf.RoundToInt(tuppos.x * Resolution), Mathf.RoundToInt(tuppos.y * Resolution), Mathf.RoundToInt(tuppos.z * Resolution));
                if (sceenpoints.Contains(tup)) continue;
                sceenpoints.Add(tup);
                
                 // vertexColor
                 var vertexColor = printItem.mesh.colors[i];
                 Vector3 vertexColorHSV = Vector3.one;
                 Color.RGBToHSV(vertexColor, out vertexColorHSV.x, out vertexColorHSV.y, out vertexColorHSV.z);//HSV = xyz, h=x, s=y, v=z
                
                // Add finalized printer command
                printerCommands.Add(new PrinterMem.PrinterCommand(tuppos, printerCommands.Count, vertexColorHSV));
                
            }
        }
    }
    private void OnDrawGizmos()
    {
        if(transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
            Debug.Log("The TAP ghost system does not take into account nested scaled objects. Please scale the child objects instead!");
        }

        #region prepare meshes 
        Transform[] ChildArray = transform.GetComponentsInChildren<Transform>();


        //Clean up nested scaling
        foreach (Transform item in ChildArray)
        {
            if(item.childCount > 0)
            {
                if(item.localScale != Vector3.one)
                {
                    item.localScale = Vector3.one;
                    Debug.Log("The TAP ghost system does not take into account nested scaled objects. Please scale the child objects instead!" + "Set scaling Vector3(1,1,1) for : " + item.name);
                }
            }

        }

        //If new number of children does not match previous frames data
        if (itemLength != ChildArray.Length-1)
        {
            itemLength = ChildArray.Length-1;
            if (itemLength == 0)
            {
                SetUpPrintObjects();
                Debug.Log("There are no mesh renderers to pull Vertex data from");
            }
            else
            {
                Debug.Log("Child mesh renderer array length changed to: " + itemLength);
                SetUpPrintObjects();
            }
        }
        #endregion
        
        #region Ghost Gizmo sim

        //Calculate the Vertex Count
        TotalVertsInMeshArray = 0;//total number of verts (unmodifired)
         TotalVertsInMeshArrayFinal = 0;//total number of verts final
         foreach (var item in PrintItems)
         {
            if(item == null)
            {
                PrintItems.Remove(item);
                continue;
            }


            //Vert Count
           TotalVertsInMeshArray += item.mesh.vertexCount;//Add verts to TotalVertsInMeshArray
           //TotalVertsInMeshArrayFinal += (int)Math.Round(item.mesh.vertexCount * (1 - item.PercentVertsToSkip));//percent per object
           item.GhostBounds(GhostColor, OutOfBoundsColor, SelectedColor);//sim Ghost bounding box and ghost wireframe
         }
        //TotalVertsInMeshArrayFinal = (int)Math.Round(TotalVertsInMeshArrayFinal * (1 - PercentOfVertsToSkipMultiplier));//percent overall
        TotalVertsInMeshArrayFinal = printerCommands.Count;



        #endregion
        //Draw Bounds Cube
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 2);

    }

}
