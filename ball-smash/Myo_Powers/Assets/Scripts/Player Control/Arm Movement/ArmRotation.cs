using UnityEngine;
using System.Collections;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class ArmRotation : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    private bool firstTime = true;
    public GameObject InstructionUI;

    [Header ("Myo Armband Object")]
    public GameObject myo = null;

    [Header ("Arm Objects")]
    public GameObject foreArmBone;
    public GameObject armGameObjectHolder;

    [Header("Arm Rotation Smoothing")]
    public float rollSmoothing = 2;
    public float armXYSmoothing = 2;

    [Header("Arm Roll Settings")]
    public float maxNegArmRoll = 90;
    public float maxPosArmRoll = 90;
    public float maxNegMyoRoll = 0;
    public float maxPosMyoRoll = 35;

    private float myoMinRollAngle = 0;
    private float myoMaxRollAngle = 0;
    private float myoRollRange = 0;
    private float armRollRange = 0;
    private float rollMultiplier = 0;

    private float currentMyoRoll = 0;

    private float normalBoneRoll = 0;
    private float myoCalibrationRoll = 0;
    private float currentArmRoll = 0;

    [Header ("Arm Up/Down (XY) Rotation Settings")]
    public float maxYRot = 0;
    public float minYRot = 0;
    public float maxXRot = 0;
    public float minXRot = 0;

    private float maxMyoYAngle = 0;
    private float minMyoYAngle = 0;
    private float maxMyoXAngle = 0;
    private float minMyoXAngle = 0;

    private float normalBoneX = 0;
    private float normalBoneY = 0;

    private float myoCalibrationX = 0;
    private float myoCalibrationY = 0;

    private float currentMyoX = 0;
    private float currentMyoY = 0;

    private float currentArmHolderX = 0;
    private float currentArmHolderY = 0;
    private float normalArmHolderX = 0;
    private float normalArmHolderY = 0;
    private float normalArmHolderZ = 0;

    private bool canControlArm = true;
    public static bool hasBeenCalibrated = false;

    Vector3 newArmRotationLocal = Vector3.zero;
    Vector3 newArmHolderRotationLocal = Vector3.zero;

    ThalmicMyo myoArmband;

    void Start()
    {
        //Get myo compononent
        myoArmband = myo.GetComponent<ThalmicMyo> ();

        //Set default values of the fore arm bone rotation at start
        SetDefaultRollValues ();

    }

    void Update()
    {
        if(firstTime && hasBeenCalibrated)
        {
            InstructionUI.SetActive (false);
            firstTime = false;
        }

        //If arm is not outside the boundaries ( angle ) then in game arm receives updates
        if (myo)
        {
            //If canControlArm and hasBeenCalibrated is true, then update arm
            if(canControlArm && hasBeenCalibrated)
            {
                UpdateRoll ();
                UpdateXYRotation ();
                SetArmRotation ();
            }
        }

        //Calibrate myo with the in game arm when needed
        if (myoArmband.pose == Pose.DoubleTap)
        {
            CalibrateMyoWithGameArm ();
            hasBeenCalibrated = true;   //First play through requires calibration
        }

        //Debug.Log (currentMyoX);
        //print (myoArmband.pose);
        //print (currentMyoRoll);
    }

    /// <summary>
    /// Update the roll of the arm (Forward axis rotation for the forearm bone)
    /// </summary>
    void UpdateRoll()
    {
        currentMyoRoll = GetCurrentMyoRollWithinMaxRange ();

        //Roll the arm based on currentMyoRoll X rollmutliplier
        currentArmRoll = (normalBoneRoll + 90) - (currentMyoRoll * rollMultiplier);

        //Set new arm roll (With added smoothing)
        float tempFloat = newArmRotationLocal.x;
        newArmRotationLocal.x = Mathf.Lerp (tempFloat, currentArmRoll, Time.deltaTime * rollSmoothing);
    }

    void UpdateXYRotation()
    {
        UpdateCurrentMyoXYRotationsWithinRange ();

        //Set new arm x rotation
        newArmHolderRotationLocal.x = Mathf.Lerp (newArmHolderRotationLocal.x, currentMyoX + currentArmHolderX, Time.deltaTime * armXYSmoothing);

        newArmHolderRotationLocal.y = Mathf.Lerp (newArmHolderRotationLocal.y, currentMyoY + currentArmHolderY, Time.deltaTime * armXYSmoothing);

    }

    void SetArmRotation()
    {
        foreArmBone.transform.localEulerAngles = newArmRotationLocal;
        armGameObjectHolder.transform.localEulerAngles = newArmHolderRotationLocal;
    }

    /// <summary>
    /// Get current myo roll within the defined range
    /// </summary>
    /// <returns></returns>
    float GetCurrentMyoRollWithinMaxRange()
    {
        float roll = 0;

        if(myoArmband.transform.rotation.eulerAngles.z <= 360 && myoArmband.transform.rotation.eulerAngles.z >= 180)
        {
            if(myoArmband.transform.rotation.eulerAngles.z >= myoMaxRollAngle)
            {
                roll = myoRollRange;
            }
            else if(myoArmband.transform.rotation.eulerAngles.z <= myoMinRollAngle)
            {
                roll = 0;
            }
            else
            {
                roll = myoArmband.transform.rotation.eulerAngles.z - myoMinRollAngle;
            }
        }

        return roll;
    }

    void UpdateCurrentMyoXYRotationsWithinRange()
    {
        Vector3 currentMyoRotations = myoArmband.transform.localRotation.eulerAngles;

        if(currentMyoRotations.x <= 360 && currentMyoRotations.x >= 270)
        {
            currentMyoX = currentMyoRotations.x - 360;
            currentMyoX -= myoCalibrationX;
        }
        else
        {
            currentMyoX = currentMyoRotations.x - myoCalibrationX;
        }

        if(currentMyoX <= -maxXRot)
        {
            currentMyoX = -maxXRot;
            //Debug.Log ("MaxAngle Reached");
        }
        else if(currentMyoX >= minXRot)
        {
            currentMyoX = minXRot;
            //Debug.Log ("MinAngle Reached");
        }



        //currentMyoY = currentMyoRotations.y - myoCalibrationY;

        if (currentMyoRotations.y <= 360 && currentMyoRotations.y >= 270)
        {
            currentMyoY = currentMyoRotations.y - 360;
            currentMyoY -= myoCalibrationY;
        }
        else
        {
            currentMyoY = currentMyoRotations.y - myoCalibrationY;
        }

        if (currentMyoY >= maxYRot)
        {
            currentMyoY = maxYRot;
        }
        else if(currentMyoY <= -minYRot)
        {
            currentMyoY = -minYRot;
        }
       
    }

    void SetDefaultRollValues()
    {
        //These values will not change during runtime
        newArmRotationLocal.y = foreArmBone.transform.localEulerAngles.y;
        newArmRotationLocal.z = foreArmBone.transform.localEulerAngles.z;

        normalBoneX = foreArmBone.transform.localEulerAngles.z;
        normalBoneY = foreArmBone.transform.localEulerAngles.y;

        normalArmHolderX = armGameObjectHolder.transform.localEulerAngles.x;
        normalArmHolderY = armGameObjectHolder.transform.localEulerAngles.y;
        normalArmHolderZ = armGameObjectHolder.transform.localEulerAngles.z;

        //maxYAngle = normalBoneY + maxYRot;
        //minYAngle = normalBoneY - minYRot;
        //maxXAngle = normalBoneX + maxXRot;
        //minXAngle = normalBoneX - minXRot;

        //print (minXAngle);

        //normalBoneRoll = x axis because bone forward is on the x axis
        normalBoneRoll = foreArmBone.transform.localEulerAngles.x;

        armRollRange = maxNegArmRoll + maxPosArmRoll;
        myoRollRange = maxPosMyoRoll + maxNegMyoRoll;
        rollMultiplier = armRollRange / myoRollRange;


    }

    /// <summary>
    /// Calibrate the myo with the arm on screen
    /// </summary>
    void CalibrateMyoWithGameArm()
    {
        //Current myo rotations at calibration function call
        myoCalibrationRoll = myoArmband.transform.localRotation.eulerAngles.z;
        myoCalibrationX = myoArmband.transform.localRotation.eulerAngles.x;
        myoCalibrationY = myoArmband.transform.localRotation.eulerAngles.y;
        currentArmHolderX = normalArmHolderX;
        currentArmHolderY = normalArmHolderY;

        //Myo x rotation in up direction leads through 360 -> 180, not 0 -> 180 (also same as 0 -> -90)
        //Get max myo x rotation (up) and min (down)
        if (myoCalibrationX >= 270 && myoCalibrationX <= 360)
        {
            myoCalibrationX = myoCalibrationX - 360;

            maxMyoXAngle = myoCalibrationX - maxXRot;
            minMyoXAngle = myoCalibrationX + minXRot;

            //Debug.Log (maxMyoXAngle + "   " + minMyoXAngle);
        }
        else
        {
            maxMyoXAngle = myoCalibrationX - maxXRot;
            minMyoXAngle = myoCalibrationX + minXRot;

            //Debug.Log (maxMyoXAngle + "   " + minMyoXAngle);
        }

        //Get max myo y rotation (right) and min (left)
        maxMyoYAngle = myoCalibrationY + maxYRot;
        minMyoYAngle = myoCalibrationY - minYRot;


        myoMaxRollAngle = myoCalibrationRoll + (myoRollRange / 2);
        myoMinRollAngle = myoCalibrationRoll - (myoRollRange / 2);

        //Reset arm position in game to default rotations
        newArmRotationLocal.x = normalBoneRoll;
        newArmRotationLocal.y = normalBoneY;
        newArmRotationLocal.z = normalBoneX;
        foreArmBone.transform.localEulerAngles = newArmRotationLocal;

        newArmHolderRotationLocal.x = normalArmHolderX;
        newArmHolderRotationLocal.y = normalArmHolderY;
        newArmHolderRotationLocal.z = normalArmHolderZ;
        armGameObjectHolder.transform.localEulerAngles = newArmHolderRotationLocal;
    }
}
