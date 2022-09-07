using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrinterMirror : MonoBehaviour
{
    public enum DebugPointType {GameObject, Particle};
    public PrinterMem.PrinterCommand printerCommand;
    [Header("Options")]
    public bool IsEnabled = true;
    [Help("DebugPointType.Particle is recommended")]
    public DebugPointType debugPointType = DebugPointType.Particle;
    [Header("Required Ref")]
    public GameObject PointPrefab;//Prefab sphere
    private Transform MirrorObjectStorage;
    public ParticleSystem MirrorParticleSystem;
    public Animator MirrorParticleAnimator;
    public void XYZNuzzleMirror(PrinterMem.PrinterCommand printcommand)
    {
        if (!IsEnabled) return;
        printerCommand = printcommand;
    }
    public void ParticleTriggerOn()
    {
        if (!IsEnabled) return;
        if (debugPointType == DebugPointType.Particle) SpawnParticle();
        if (debugPointType == DebugPointType.GameObject) SpawnGameObject();
    }
    public void SpawnGameObject()
    {
        if (MirrorObjectStorage == null)
        {
            MirrorObjectStorage = new GameObject().transform;
            MirrorObjectStorage.name = "MirrorObjectStorage";
        }
        Transform point = Instantiate(PointPrefab).transform;
        point.position = printerCommand.pos;
        point.transform.SetParent(MirrorObjectStorage);
        point.gameObject.name = "" +Round(printerCommand.HSV.x);
        //point.gameObject.name = "" + printerCommand.PointID + "(" + Round(printerCommand.HSV.x) + "," + Round(printerCommand.HSV.y)+","+ Round(printerCommand.HSV.z)+")";

    }
    public float Round(float num)
    {
        return Mathf.Round(num * 10.0f) * 0.1f; 
    }
    [SerializeField] [ReadOnlyInspector] private bool EmitNextFrame = false;
    public void SpawnParticle()
    {
        //Color
        MirrorParticleAnimator.SetFloat("H", printerCommand.HSV.x);
        MirrorParticleAnimator.SetFloat("S", printerCommand.HSV.y);
        MirrorParticleAnimator.SetFloat("V", printerCommand.HSV.z);
        //Pos
        MirrorParticleSystem.transform.position = printerCommand.pos;
        EmitNextFrame = true;

    }
    public void Update()
    {
        if(EmitNextFrame == true)
        {
            EmitNextFrame = false;
            MirrorParticleSystem.Emit(1);
        }
    }
}
