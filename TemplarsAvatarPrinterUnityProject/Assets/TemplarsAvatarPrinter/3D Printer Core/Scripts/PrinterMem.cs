using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PrinterMem : MonoBehaviour
{
    
    public enum OrderType { HierarchyObjectOrder, Random, BottomToTop };
    [Header("Options (Order particles are placed)")]
    [Tooltip("Reorders the ghostManager's total set of vertex's")] public OrderType orderType = OrderType.BottomToTop;
    [Header("Required Ref")]
    public GhostManager ghostManager;
    public PrinterCPU printerCPU;
    public PrintSequenceManager printSequenceManager;
    [Header("Data")]
    [Tooltip("The current vertex we will be printing")] [ReadOnlyInspector] public int SelectedSlot;
    [Tooltip("The total list of XYZ commands we have")] [ReadOnlyInspector] public int XYZListCount;
    [ReadOnlyInspector] public List<PrinterCommand> XYZList;

    public void Start()//This is where the printer starts
    {
        if(ConvertFromGhostsToPrinterCommandList() == true)
        {
            Debug.Log("Logged Printer Mem into printerMem");
            printerCPU.StartPrint();
        }
        else
        {
            printSequenceManager.StartNext3DPrint();
        }

    }
    public struct PrinterCommand
    {
        public Vector3 pos;//xyz of the particle
        public int PointID;//place in the array
        public Vector3 HSV;
        public PrinterCommand(Vector3 temppos, int pointID, Vector3 hSV)
        {
            this.pos = temppos;
            this.PointID = pointID;
            this.HSV = hSV;
        }
    }
    public bool RequestAndIncreaseNextSlot()
    {
        if (SelectedSlot >= XYZList.Count - 1) return false;//End of the array
        SelectedSlot++;
        return true;//We still got work to do
    }
    
    
    public bool ConvertFromGhostsToPrinterCommandList()
    {
        SelectedSlot = 0;
        ghostManager.CalculatePrinterCommands();
        XYZList = ghostManager.printerCommands.ToList();
        XYZListCount = XYZList.Count;
        if(XYZListCount == 0)
        {
            Debug.LogWarning("There are no verticies logged in the current print item");
            return false;//Nothing to organize
        }
        if (orderType == OrderType.BottomToTop)
        {
            XYZList = XYZList
                        .OrderBy(x => x.pos.y)
                        .ThenBy(x => x.pos.x)
                        .ThenBy(x => x.pos.z)
                        .ToList();
        }
        if (orderType == OrderType.Random)
        {
            XYZList.Shuffle();
        }
        return true;//was able to organize
    }
}
