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
        public bool isHolding { get; set; }
    private float triggered;
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
            triggered = OVRInput.Get(f.Input);

            if (isHolding) triggered = 1;

            f.Pointer.localPosition = new Vector3(
                Mathf.Lerp(f.StartPos.x, f.EndPos.x, triggered),
                Mathf.Lerp(f.StartPos.y, f.EndPos.y, triggered),
                Mathf.Lerp(f.StartPos.z, f.EndPos.z, triggered));


        }
    }
}
