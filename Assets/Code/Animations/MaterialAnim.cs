using UnityEngine;

public class MaterialAnim : MonoBehaviour
{
    public Material Mat;
    public Texture[] TEX;
    private float Timer;
    public float Delay = 0.1f;
    public float CycleDelay = 0.1f;
    private int num;

    public bool RND = true;
    void Update()
    {
        Mat.mainTexture = TEX[num];
        if (Timer >= Time.fixedTime) return;
        
        if (RND)
        {

            num = Random.Range(0, TEX.Length);
            Timer = Time.fixedTime + Delay;
            return;
        }
           
        if (num < TEX.Length - 1)
        {
            num++;
            Timer = Time.fixedTime + Delay;
        }
        else
        {
            num = 0;
            Timer = Time.fixedTime + CycleDelay;

        }

    }

}
