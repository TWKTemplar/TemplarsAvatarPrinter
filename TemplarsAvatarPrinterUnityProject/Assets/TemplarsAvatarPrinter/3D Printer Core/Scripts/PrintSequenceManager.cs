using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintSequenceManager : MonoBehaviour
{
    [Header("Required Ref")]
    public GhostManager ghostManager;
    public PrinterMem printerMem;
    public PrinterMirror printerMirror;
    public PrinterCPU printerCPU;
    [Help("PrintSequenceManager is only used when PrinterCPU.NumberOfPrints = MultiplePrints")]
    [Header("Data")]
    [ReadOnlyInspector]public int CurrentMesh;
    public Mesh[] MeshArray;
    public MeshFilter TargetMeshFilterToSwapOut;

    public void Start()
    {
        if (TargetMeshFilterToSwapOut == null) TargetMeshFilterToSwapOut = ghostManager.PrintItems[0].meshFilter;
    }
    private void ClearPrint()
    {
        printerMirror.ClearMirroredPoints();
    }
    public bool IsThereAnotherPrint()
    {
        
        if (CurrentMesh+1== MeshArray.Length)
        {
            return false;
        }


        return true;
    }
    private void LoadNext3DPrint()
    {
        CurrentMesh++;
        ClearPrint();
        TargetMeshFilterToSwapOut.mesh = MeshArray[CurrentMesh];
        TargetMeshFilterToSwapOut.gameObject.name = MeshArray[CurrentMesh].name;
        ghostManager.SetUpPrintObjects();//updates the Ghost Mirrors to cache Mesh mesh  for use in CalculatePrinterCommands();
        ghostManager.CalculatePrinterCommands();

        if (printerMem.ConvertFromGhostsToPrinterCommandList() == false)
        {
            Invoke("LoadNext3DPrint", 0.25f);
        }
        else
        {
            SetPrinterToIdle();//allows for the next cycle to take place
        }
        
       
    }
    public void StartNext3DPrint()
    {
        Invoke("LoadNext3DPrint", 1);
    }
    private void SetPrinterToIdle()
    {
        printerCPU.printerState = PrinterCPU.PrinterState.idle;//allows for the next cycle to take place

    }
}
