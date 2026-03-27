using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLen = 0.4f;
    public int segmentLength = 30;
    private int segmentmax = 30;
    public float lineWidth = 0.01f;

    public Transform StartEnd;
    public Transform FinishEnd;

    public bool Dragging { get; set; }

    private Vector3 PrevPos,TargetPrevPos;
    private float draggingtimer;
    private float SimulationTimer, SimulationStartTimer;
    // Use this for initialization
    void Start()
    {
        if (StartEnd == null) Destroy(gameObject);

        this.lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        for (int i = 0; i < segmentLength; i++)
        {
            this.ropeSegments.Add(new RopeSegment(ropeStartPoint));
          //  ropeStartPoint.y -= ropeSegLen;
        }

        if (FinishEnd == null) print("NULL");

        SimulationStartTimer = Time.fixedTime + 1;


    }

    // Update is called once per frame
    void Update()
    {
        if (StartEnd == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (FinishEnd != null)
            {
                /*if ((Mathf.Abs(FinishEnd.position.x - StartEnd.position.x) + Mathf.Abs(FinishEnd.position.y - StartEnd.position.y) + Mathf.Abs(FinishEnd.position.z - StartEnd.position.z))/3 > segmentLength * ropeSegLen)
                {

                    if (segmentLength < segmentmax)
                        segmentLength++;
                    this.ropeSegments.Add(new RopeSegment(FinishEnd.position));
                    
                }*/

             
            }
            else
            {
                segmentLength = 5;

                for (int i = 0; i < segmentmax; i++)
                {
                    if (this.ropeSegments.Count > 5)
                        this.ropeSegments.RemoveAt(ropeSegments.Count - 1);
                }
            }

           
        }

       
        if (SimulationTimer < Time.fixedTime || SimulationStartTimer>Time.fixedTime)
        {
            if (StartEnd != null)
            {
                Simulate();
                DrawRope();
            }


            SimulationTimer = Time.fixedTime + 3;
        }


        Vector3 FinishPos = Vector3.zero;
        Vector3 StartPos = Vector3.zero;

        if (StartEnd != null)
        {
            StartPos = StartEnd.position;
            
        }

        if (FinishEnd != null)
        {
            FinishPos = FinishEnd.position;
        }

        if (PrevPos != StartPos || TargetPrevPos != FinishPos)
        {
            if (SimulationTimer < Time.fixedTime)
            {
               // Simulate();
               // DrawRope();

                SimulationTimer = Time.fixedTime + 0.01f;
            }
            
          //  Simulate();
               
        }

        if (segmentLength == segmentmax)
        {
            if (Mathf.Abs(StartPos.x - FinishPos.x) > ropeSegLen * (segmentLength-2)) ropeSegLen = Mathf.Lerp(ropeSegLen, (Mathf.Abs(StartPos.x - FinishPos.x) / segmentLength) * 1.2f, Time.deltaTime / 2);

        }

        if (draggingtimer < Time.fixedTime)
        {
                TargetPrevPos = FinishPos;

            
            PrevPos = StartPos;
            draggingtimer = Time.fixedTime + 0.1f;
        }

        
    }

    private void Simulate()
    {
        
        // SIMULATION
        Vector3 forceGravity = new Vector3(0f, -30.5f,0f);

        for (int i = 1; i < this.segmentLength; i++)
        {
            RopeSegment firstSegment = this.ropeSegments[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.fixedDeltaTime;
            this.ropeSegments[i] = firstSegment;
        }

        //CONSTRAINTS
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        //Constrant to Mouse
       /* RopeSegment firstSegment = this.ropeSegments[0];
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.ropeSegments[0] = firstSegment;*/

        RopeSegment firstSegment = ropeSegments[0];

        if(StartEnd!=null)
        firstSegment.posNow = StartEnd.position;
        ropeSegments[0] = firstSegment;
        ropeSegments[ropeSegments.Count-1] = firstSegment;




        if (FinishEnd != null)
        {
            RopeSegment endSegment = ropeSegments[segmentLength-1];
            endSegment.posNow = FinishEnd.position;
            
            ropeSegments[segmentLength-1] = endSegment;
        }

        for (int i = 0; i < ropeSegments.Count; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];

            RopeSegment secondSeg;

            if(i< ropeSegments.Count-1)
                secondSeg = ropeSegments[i + 1];
            else secondSeg = ropeSegments[ropeSegments.Count - 1];


            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - this.ropeSegLen);
            Vector3 changeDir = Vector3.zero;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector3 changeAmount = changeDir * error;
            if (i != 0)
            {
                if (i < ropeSegments.Count - 1)
                {
                    firstSeg.posNow -= changeAmount * 0.5f;
                    ropeSegments[i] = firstSeg;
                    secondSeg.posNow += changeAmount * 0.5f;
                    ropeSegments[i + 1] = secondSeg;
                }
                else
                {
                   
                    RopeSegment endSegment = ropeSegments[i];
                    endSegment.posNow = FinishEnd.position;
                    ropeSegments[i] = endSegment;
                    
                }


            }
            else
            {
                
                    secondSeg.posNow += changeAmount;
                    ropeSegments[i + 1] = secondSeg;
                
              
            }


        }




    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentLength];
        for (int i = 0; i < segmentLength; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector3 posNow;
        public Vector3 posOld;

        public RopeSegment(Vector3 pos)
        {
           posNow = pos;
           posOld = pos;
        }
    }
}