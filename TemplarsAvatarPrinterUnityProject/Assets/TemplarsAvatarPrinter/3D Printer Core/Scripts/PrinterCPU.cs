using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterCPU : MonoBehaviour
{
    public enum XYZSyncSpeed { FastestSpeedForUnity, FastSpeedForVRChatClientSide, NormalSpeedForVRChatMultiplayer, }
    [Header("Options (Debug focused)")]
    [Tooltip("After each vertex will the program automatically place the next")] public bool AutoLoop = true;
    public enum OnPrintEnd { SinglePrint, MultiplePrints}
    [SerializeField] public OnPrintEnd NumberOfPrints;
    public enum Timelaps {NoScreenCapture, ScreenCaptureAtEndOfEveryPrint, ScreenCaptureEveryNPoints}
    [SerializeField] public Timelaps TimelapSettings;
    [SerializeField][Range(50,200)] public int PointsTillScreenshot = 100;

    [SerializeField] public XYZSyncSpeed XYZSyncSpeedSetting;
    [Tooltip("0.1f is safe for multiplayer networking, if you only care about your vrchat client side's view 0.03f is safe")]
    [ReadOnlyInspector] public float TimeBetweenXYZSync = 0.1f;
    
    [Header("Time")] 
    [Tooltip("TimeBetweenXYZSync = (TimeBetweenXYZSync * 2) * (printerMem.XYZListCount - printerMem.SelectedSlot)")]
    [ReadOnlyInspector]public string TimeRemaining;//Calculated remaning time on a print process
    [Header("Required Ref")]
    public ScreenshotManager screenshotManager;
    public PrinterMem printerMem;
    public OSCSender oSCSender;
    public PrintSequenceManager printSequenceManager;
    [Header("Data")]
    [ReadOnlyInspector] [Tooltip("READ ONLY, What the OSC program's simulated Particle emmitor is set to.")] public bool ParticleEmmiterOn = true;//READ ONLY (Does not do anything)
    [ReadOnlyInspector] private float TimeRemainingFloat;
    [ReadOnlyInspector] public bool DidStartup = false;
    public enum PrinterState { idle, ColorAndMove, PlacingParticle };
    [ReadOnlyInspector] public PrinterState printerState = PrinterState.idle;
    
    public void Awake()
    {
        CalculateXYZSpeed();
    }
    public void Update()
    {
        if (printerMem.SelectedSlot != 0) CalculateRemainingTime();
        if ( printerState == PrinterState.idle && AutoLoop == true) StartParticlePlacement();
    }
    private void OnDrawGizmos()
    {
        CalculateXYZSpeed();
    }
    public void StartPrint()
    {
        printerMem.SelectedSlot = 0;
    }
    
    #region Loop system
    public void StartParticlePlacement()
    {
        if (DidStartup == false)//Initalize avatar specific requirements
        {
            oSCSender.WorldConstraintOff();//Leaves the world constraint in front of me
            Invoke("ColorAndMove", TimeBetweenXYZSync);
        }
        else // Continue as normal
        {
            ColorAndMove();
        }
    }
    public void ColorAndMove()//color and move
    {
      
        printerState = PrinterState.ColorAndMove;//Blocks a new Cycle from starting
        ParticleEmmiterOn = false;//READONLY for inspector debuging


        oSCSender.ParticleTriggerOff();//gets ready to place a new particle
        if(printerMem.XYZList.Count != 0) oSCSender.SendXYZDataToVRC(printerMem.XYZList[printerMem.SelectedSlot]);//Move the nozzle, pass along PrinterColorMode
        Invoke("PlaceParticle", TimeBetweenXYZSync);
    }
    public void PlaceParticle()//place particle
    {
        printerState = PrinterState.PlacingParticle;

        ParticleEmmiterOn = true;//READ ONLY (Does not do anything)
        if(DidStartup == false)
        {
            DidStartup = true;
            oSCSender.ParticleBufferOn();//Requests that the particle buffer is active, will place a single particle on start
            Invoke("StartParticlePlacement", TimeBetweenXYZSync); //In order to place the 0th particle on start up we attempt to place the 1st particle again after the Particle buffer has been turned on.
        }
        else
        {
            oSCSender.ParticleTriggerOn();//Places the particle point into the world
            Invoke("LoadNextParticle", TimeBetweenXYZSync);
        }
       
    }
    public void LoadNextParticle()
    {

        if (TimelapSettings == Timelaps.ScreenCaptureEveryNPoints)
        {
            PointsTillScreenshot--;
            if(PointsTillScreenshot <= 0)
            {
                PointsTillScreenshot = 100;
                screenshotManager.TakeScreenShot();
            }
        }
        if (printerMem.RequestAndIncreaseNextSlot())//bool that returns false if we printed everything
        {
            printerState = PrinterState.idle;//allows for the next cycle to take place
        }
        else
        {
            if (NumberOfPrints == OnPrintEnd.SinglePrint)
            {
                oSCSender.ENDPROGRAM();
            }
            if (NumberOfPrints == OnPrintEnd.MultiplePrints)
            {
                if (TimelapSettings != Timelaps.NoScreenCapture) screenshotManager.TakeScreenShot();
                if(printSequenceManager.IsThereAnotherPrint() == true)
                {
                    printSequenceManager.StartNext3DPrint();
                }
                else
                {
                    oSCSender.ENDPROGRAM();
                }
            }
        }
    }
    #endregion  
    public void CalculateXYZSpeed()
    {
        #region XYZSyncSpeedSetting
        switch (XYZSyncSpeedSetting)
        {
            case XYZSyncSpeed.NormalSpeedForVRChatMultiplayer: TimeBetweenXYZSync = 0.1f; break;//slow
            case XYZSyncSpeed.FastSpeedForVRChatClientSide: TimeBetweenXYZSync = 0.03f; break;//med
            case XYZSyncSpeed.FastestSpeedForUnity: TimeBetweenXYZSync = 0f; break;//fast
            default: break;
        }
        #endregion
    }
    public void CalculateRemainingTime()
    {
        TimeRemainingFloat = (TimeBetweenXYZSync * 2) * (printerMem.XYZListCount - printerMem.SelectedSlot);

        if(TimeBetweenXYZSync == 0)
        {
            TimeRemainingFloat = (0.01f * 2) * (printerMem.XYZListCount - printerMem.SelectedSlot);//This is so that there will be a timer even when TimeBetweenXYZSync = 0
        }
        int minutes = (int)TimeRemainingFloat / 60;
        int seconds = (int)((TimeRemainingFloat) % 60);
        TimeRemaining = $"{minutes}:{seconds:00}" + " @ " + Mathf.RoundToInt(((float)printerMem.SelectedSlot / (float)printerMem.XYZListCount) * 100) + "%";
    }
}
