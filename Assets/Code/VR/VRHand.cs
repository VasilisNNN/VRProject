using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Finger
{
    public Transform Pointer;
    public Transform Target;

 
    public OVRInput.Axis1D Input;


    public Vector3 StartPos;
    public Vector3 EndPos;
}
public class VRHand : MonoBehaviour
{
    public List<Finger> Fingers = new List<Finger>();

    private void Start()
    {
        foreach (Finger f in Fingers)
        {
            f.StartPos = f.Pointer.localPosition;
            f.EndPos = f.Target.localPosition;
        }
    }
    void Update()
    {

        foreach (Finger f in Fingers)
        {
            f.Pointer.localPosition = new Vector3(
                Mathf.Lerp(f.StartPos.x, f.EndPos.x, OVRInput.Get(f.Input)),
                Mathf.Lerp(f.StartPos.y, f.EndPos.y, OVRInput.Get(f.Input)),
                Mathf.Lerp(f.StartPos.z, f.EndPos.z, OVRInput.Get(f.Input)));


        }
    }
}
