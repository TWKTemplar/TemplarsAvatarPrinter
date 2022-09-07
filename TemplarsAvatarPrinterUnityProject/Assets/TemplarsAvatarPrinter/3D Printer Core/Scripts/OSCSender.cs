using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscCore;
using UnityEngine.UI;

/// <summary>
/// The OSCSender Class Works as a Network API for the Printer CPU 
/// </summary>
public class OSCSender : MonoBehaviour
{

    [Header("Optional Ref")]
    public PrinterMirror printerMirror;
    [Header("OSC Networking Setup")]
    public OscClient client;
    public OscServer server;


    void Start()
    {
        client = new OscClient("127.0.0.1", 9000);
        server = new OscServer(9001);
    }

    #region Send XYZ Data
    public void SendXYZDataToVRC(PrinterMem.PrinterCommand pntCmd)
    {
        if (printerMirror != null) printerMirror.XYZNuzzleMirror(pntCmd);

        //Send HSV Data
        client.Send("/avatar/parameters/H", pntCmd.HSV.x);
        client.Send("/avatar/parameters/S", pntCmd.HSV.y);
        client.Send("/avatar/parameters/V", pntCmd.HSV.z);


        //Send XYZ Data
        client.Send("/avatar/parameters/X", pntCmd.pos.x);
        client.Send("/avatar/parameters/Y", pntCmd.pos.y);
        client.Send("/avatar/parameters/Z", pntCmd.pos.z);
    }

    #endregion

    #region Particle Spanwer and Buffer Trigger Calls

    public void ParticleTriggerOff()
    {
        client.Send("/avatar/parameters/ParticleTrigger", false);
    }
    public void ParticleTriggerOn()
    {
        if(printerMirror != null) printerMirror.ParticleTriggerOn();
        client.Send("/avatar/parameters/ParticleTrigger", true);
    }
    public void ParticleBufferOn()
    {
        if (printerMirror != null) printerMirror.ParticleTriggerOn();//Once a particle buffer is turned on they fire out a single particle
        client.Send("/avatar/parameters/ParticleBuffer", true);
    }
    public void ParticleBufferOff()
    {
        client.Send("/avatar/parameters/ParticleBuffer", false);
    }
    public void WorldConstraintOn()
    {
        client.Send("/avatar/parameters/WorldConstraint", true);
    }
    public void WorldConstraintOff()
    {
        client.Send("/avatar/parameters/WorldConstraint", false);
    }
    #endregion

    #region End Program
    public void ENDPROGRAM()
    {
        ParticleTriggerOff();
        Invoke("ENDPROGRAMFINAL", 1);
    }
    private void ENDPROGRAMFINAL()
    {
           #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
            #else
         Application.Quit();
            #endif
    }
    #endregion
}
