using UnityEngine;
using System.Collections;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class MyoPoseCheck : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    public bool isPoseCheckEnabled = true;

    public GameObject armAnimationObject;

    [Header ("Myo")]
    public GameObject myoGameObject;

    [Header ("Settings")]
    public float checkNewMyoPoseRate = 1f;

    private float nextMyoPoseCheck = 0f;

    private bool isLightningOn = false;
    //private bool isFireOn = false;
    private bool isGripOn = false;
    private bool isPushOn = false;
    private bool isPullOn = false;

    Pose lastMyoPose;

    ThalmicMyo myo;

    Animator armAnimator;

    public delegate void PoseAction();
    public static event PoseAction onUseLightning;
    public static event PoseAction onStopLightning;
    public static event PoseAction onUseFire;
    public static event PoseAction onStopFire;
    public static event PoseAction onUsePush;
    public static event PoseAction onUsePull;
    public static event PoseAction onUseGrip;
    public static event PoseAction onStopGrip;

    void Start()
    {
        myo = myoGameObject.GetComponent<ThalmicMyo> ();
        armAnimator = armAnimationObject.GetComponent<Animator> ();
    }

    void Update()
    {
        if(isPoseCheckEnabled)
        {
            GetCurrentPose ();
            GetPower ();
        }
    }

    void GetCurrentPose()
    {
        if(Time.time > nextMyoPoseCheck)
        {
            lastMyoPose = myo.pose;

            nextMyoPoseCheck = Time.time + checkNewMyoPoseRate;
        }
    }

    void GetPower()
    {
        if(ArmRotation.hasBeenCalibrated)
        {

            switch(myo.pose)
            {
                case Pose.FingersSpread:
                    if(onUseLightning != null)
                    {
                        onUseLightning ();
                        isLightningOn = true;

                        armAnimator.SetBool ("lightning", true);
                    }

                    StopUsingGrip ();
                    //StopUsingFire ();
                    StopUsingPush ();
                    StopUsingPull ();
                    break;
                case Pose.Fist:
                    if(onUseGrip != null)
                    {
                        onUseGrip ();
                        isGripOn = true;

                        armAnimator.SetBool ("grip", true);
                    }

                    //StopUsingFire ();
                    StopUsingLightning ();
                    StopUsingPush ();
                    StopUsingPull ();
                    break;
                case Pose.WaveIn:
                    if (onUsePull != null)
                    {
                        if (isPullOn != true)
                        {
                            onUsePull ();
                            isPullOn = true;

                            armAnimator.SetTrigger ("wavein");
                        }

                        
                    }

                    //StopUsingFire ();
                    StopUsingGrip ();
                    StopUsingLightning ();
                    StopUsingPush ();
                    break;
                case Pose.WaveOut:
                    if(onUsePush != null)
                    {
                        if(isPushOn != true)
                        {
                            onUsePush ();
                            isPushOn = true;

                            armAnimator.SetTrigger ("waveout");
                        }
                    }

                    //StopUsingFire ();
                    StopUsingGrip ();
                    StopUsingLightning ();
                    StopUsingPull ();
                    break;
                case Pose.DoubleTap:
                case Pose.Rest:
                case Pose.Unknown:
                    //StopUsingFire ();
                    StopUsingGrip ();
                    StopUsingLightning ();
                    StopUsingPush ();
                    StopUsingPull ();
                    break;
            }
        }
    }

    void StopUsingLightning()
    {
        if(isLightningOn)
        {
            onStopLightning ();
            isLightningOn = false;
            armAnimator.SetBool ("lightning", false);
        }
    }

    /*void StopUsingFire()
    {
        if(isFireOn)
        {
            onStopFire ();
            isFireOn = false;
        }
    }*/

    void StopUsingGrip()
    {
        if(isGripOn)
        {
            onStopGrip ();
            isGripOn = false;
            armAnimator.SetBool ("grip", false);
        }
    }

    void StopUsingPush()
    {
        isPushOn = false;
    }

    void StopUsingPull()
    {
        isPullOn = false;
    }
}
