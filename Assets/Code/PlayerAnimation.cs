using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
public class PlayerAnimation : MonoBehaviour
{
    private Animator BodyAnim;
    private Player pl;
    private InputMode IM;
    private Menu _Menu;
    public Gun _Gun { get; set; }
    private float AnimTransDelay;
    private string CurrentAnimation;
    public float AnimationPause { get; private set; }

    private GameObject AnimatedPickedObject;
    public Transform AnimatedPickedObjectParrent;
    private Rigidbody AnimatedPORigidbody;
    private bool AnimatedOB_Setted;
    private bool FromPicked;
    void Start()
    {
    
        pl = InitializeOnAwake.pl;
        IM = InitializeOnAwake.IM;
        _Menu = InitializeOnAwake._Menu;
        _Gun = GetComponent<Gun>();


        if (pl.Body != null)
          BodyAnim = pl.Body.GetComponent<Animator>();
        
       
    }



    public void AnimManager()
    {
        if (pl._Menu.MenuONOFF || pl._gameover)
        {
            BodyAnim.speed = 0;
            return;
        }

        BodyAnim.speed = 1;

        if (AnimationPause > 0)
        {
            AnimationPause -= Time.deltaTime;
            return;
        }


        if (pl.InDialog || (pl.CutSceneMode && AnimatedPickedObject == null))
        {
 
            PlayBodyAnim("InDialog", 0.7f);
            return;
        }


        if (_Gun.ShotDurationTimer > 0) return;
        if (_Gun.ReloadTimer > 0) return;
        if (_Menu.MenuONOFF) IM.Aim = false;


        BodyAnim.SetBool("Aim", IM.Aim);


        if (pl.HandObject != null && AnimatedPickedObject == null)
        {
            if (pl.HandObject_Anim == "")
            {
                if (pl.moveDirection.magnitude * pl.Speed >= 0.1f)
                    PlayBodyAnim("HandObjectWalking", 0.1f);
                else
                    PlayBodyAnim("HandObjectStanding", 0.1f);
            }
            else
                PlayBodyAnim(pl.HandObject_Anim, 0.1f);
            return;
        }



        if (pl.moveDirection.magnitude * pl.Speed < 0.1f)
        {

            if (_Gun.GunInHandItem.itemID <= -1)
            {
            
            
                if (FromPicked)
                PlayBodyAnimNoTransition("Standing");
                else PlayBodyAnim("Standing", 0.5f);

                FromPicked = false;
                return;
            }
            if (_Gun.GunInHandItem._Guntype == Item.Guntype.knife)
            {
                PlayBodyAnim("KnifeStanding", 0.1f);
            }


            return;
        }

        if (_Gun.GunInHandItem.itemID <= -1)
        {
            PlayBodyAnim("Walking", 0.1f);
            return;
        }


        if (_Gun.GunInHandItem._Guntype == Item.Guntype.knife)
        {

            PlayBodyAnim("KnifeWalking", 0.1f);
        }
        else
        {
            print("PistolWalking");
            PlayBodyAnim("PistolWalking",0.1f);
        }


    }


    public void SetPickedAnimatedObject(string animation, GameObject obj)
    {
        if (AnimationPause > 0) return;
        PlayBodyAnim(animation,0.01f);
        AnimationPause = Mathf.Abs(GetAnimationClipLength(animation)) - 0.1f;
        print("AnimationPause " + AnimationPause);
        AnimatedPickedObject = obj;
        AnimatedPORigidbody = obj.GetComponent<Rigidbody>();
        FromPicked = true;
    }



    public void PlayBodyAnim(string animname, float fadedelay)
    {
        if (AnimationPause > 0) return;



        if (animname != CurrentAnimation)
        {
            AnimTransDelay = Time.fixedTime + fadedelay;
      
            CurrentAnimation = animname;
        }

        if (AnimTransDelay > Time.fixedTime)
        {
            BodyAnim.CrossFade(animname, fadedelay);

        }
       


    }

    public void PlayBodyAnimNoTransition(string animname)
    {
        if (animname == CurrentAnimation)
            return;


        BodyAnim.Play(animname);

        CurrentAnimation = animname;


    }

    public void AnimatedPickedObjectManager()
    {
        if (AnimatedPickedObject == null) return;

        if (AnimationPause <= 0)
        {

            if (AnimatedPickedObject == null) return;

            AnimatedPickedObject.GetComponent<DamageHeal>().UseItem();

            if (AnimatedPickedObject.GetComponent<BoxCollider>() != null)
                AnimatedPickedObject.GetComponent<BoxCollider>().isTrigger = false;

            if (AnimatedPickedObject.GetComponent<CapsuleCollider>() != null)
                AnimatedPickedObject.GetComponent<CapsuleCollider>().isTrigger = false;

            if (AnimatedPickedObject.GetComponent<MeshCollider>() != null)
                Destroy(AnimatedPickedObject.GetComponent<MeshCollider>());

            if (AnimatedPickedObject.GetComponent<Outline>() != null)
                AnimatedPickedObject.GetComponent<Outline>().enabled = true;

            AnimatedPickedObject.transform.parent = null;
            AnimatedPickedObject.transform.localScale = Vector3.one;

            if (AnimatedPORigidbody != null)
                AnimatedPORigidbody.isKinematic = false;
            AnimatedPickedObject = null;
            AnimatedOB_Setted = false;


            return;
        }



        if (!AnimatedOB_Setted)
        {
            if (AnimatedPickedObjectParrent.transform.Find(CleanObjectName(AnimatedPickedObject.name)) == null)
            {
                AnimatedPickedObject.transform.parent = AnimatedPickedObjectParrent;
                AnimatedPickedObject.transform.localPosition = Vector3.zero;
                AnimatedPickedObject.transform.localScale = Vector3.one;
                AnimatedPickedObject.transform.localEulerAngles = Vector3.one;
            }
            else
            {
                AnimatedPickedObject.SetActive(false);

            }
            AnimatedOB_Setted = true;
        }



        if (AnimatedPickedObject.GetComponent<Outline>() != null)
            AnimatedPickedObject.GetComponent<Outline>().enabled = false;

        if (AnimatedPickedObject.GetComponent<BoxCollider>() != null)
            AnimatedPickedObject.GetComponent<BoxCollider>().isTrigger = true;

        if (AnimatedPickedObject.GetComponent<CapsuleCollider>() != null)
            AnimatedPickedObject.GetComponent<CapsuleCollider>().isTrigger = true;


        if (AnimatedPickedObject.GetComponent<MeshCollider>() != null)
            Destroy(AnimatedPickedObject.GetComponent<MeshCollider>());

        if (AnimatedPORigidbody != null)
            AnimatedPORigidbody.isKinematic = true;




    }


    string CleanObjectName(string name)
    {
        // Removes "(1)", "(2)", "(34)", etc.
        return Regex.Replace(name, @"\(\d+\)$", "").Trim();
    }

    float GetAnimationClipLength(string clipName)
    {


        foreach (var clip in BodyAnim.runtimeAnimatorController.animationClips)
        {
          
            if (clip.name == clipName)
            {

                return clip.length;
            }
        }

        return -1f;
    }

}
