using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscCore;
using UnityEngine.UI;

public class OSCBase : MonoBehaviour
{
    [Header("Snazzy Code <3")]
    public bool JacketToggle;

    public float LoopSpeed = 1 ;


    //Public Variables

    [Header("OSC Networking Setup")]
    public OscClient client;
    public OscServer server;

    [Header("Object References")]
    public Text CurrentIDReadback;
    private int CurrentParameterValue;


    //Private Variables

    private bool isInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        client = new OscClient("127.0.0.1", 9000);
        server = new OscServer(9001);
        if (server.TryAddMethod("/avatar/parameters/JacketToggle", ParameterReadback))
        {
            isInitialized = true;
        }
    }

    public void ParameterReadback(OscMessageValues values)
    {
        if (!isInitialized) return;
        
        CurrentParameterValue = values.ReadIntElementUnchecked(0);
        Debug.Log($"Current Parameter Value: {CurrentParameterValue}");
    }
    [ContextMenu("StartLoop")]
    public void StartLoop()
    {
        InvokeRepeating("SendSomething", LoopSpeed, LoopSpeed);
    }
    [ContextMenu("StopLoop")]
    public void StopLoop()
    {
        CancelInvoke();
    }
    [ContextMenu("JacketToggle")]
    public void SendSomething()
    {
        if (!isInitialized) return;
        JacketToggle = !JacketToggle;
        client.Send("/avatar/parameters/ParticleSpawner", JacketToggle);
    }

    //Important, we cannot use the same readback thread as "ParameterReadback" to update our GUI Text, since its internal
    //Execution order is outside of the mainthread scope, so we have to do a "sort of" async readback
    //Inside of update
    public void Update()
    {
        CurrentIDReadback.text = $"Jacket Toggle: {CurrentParameterValue}";
    }
}
