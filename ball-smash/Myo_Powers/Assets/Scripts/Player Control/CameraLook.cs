using UnityEngine;
using System.Collections;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class CameraLook : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    [Header ("Myo")]
    public GameObject myoGameObject;

    [Header ("Camera Look Settings")]
    public GameObject playerObj;
    public GameObject cam;
    public float deadZoneBorder = 15;
    public float sensitivity = 1;
    public float maxCameraXAngle = 0;
    public float minCameraXAngle = 0;

    private float myoXAtCalibration = 0;
    private float myoYAtCalibration = 0;

    private float deadZoneXCenter = 0;
    private float deadZoneYCenter = 0;

    private bool isMouseLookEnabled = true;
    private bool isCalibrated = false;

    Vector3 newCamRot = Vector3.zero;

    ThalmicMyo myo;

    void Start()
    {
        myo = myoGameObject.GetComponent<ThalmicMyo> ();
    }

    void Update()
    {
        if(isMouseLookEnabled)
        {
            if(isCalibrated)
            {
                UpdateCamera ();
                SetNewCameraRot ();
            }
        }

        if(myo.pose == Pose.DoubleTap)
        {
            Calibrate ();

            isCalibrated = true;
        }
    }

    void UpdateCamera()
    {
        //Constantly update myo rotation
        float myoXRot = myo.transform.localRotation.eulerAngles.x;
        float myoYRot = myo.transform.localRotation.eulerAngles.y;

        //Set x deadzone boundaries
        float xUpperDeadZone = deadZoneXCenter - deadZoneBorder;
        float xLowerDeadZone = deadZoneXCenter + deadZoneBorder;

        //Convert angle to minus (for example, rather than 350 you have -10 degrees)
        if(myoXRot <= 360 && myoXRot >= 270)
        {
            myoXRot -= 360;
        }

        if (myoYRot <= 360 && myoYRot >= 270)
        {
            myoYRot -= 360;
        }

        if (myoXRot <= xUpperDeadZone)
        {
            //The farther the arm moves from dead zone the faster the camera moves with it
            float speed = myoXRot - xUpperDeadZone;
            speed = 1 * (speed / 4);

            speed *= -1;

            if (speed < 1)
            {
                speed = 1;
            }
            else if (speed >= 5)
            {
                speed = 5;
            }
            //Debug.Log (speed);

            newCamRot.x = newCamRot.x - (Time.deltaTime * sensitivity * speed);

            //Debug.Log ("Over Upper X Limit");
        }
        else if(myoXRot >= xLowerDeadZone)
        {
            //The farther the arm moves from dead zone the faster the camera moves with it
            float speed = myoXRot - xLowerDeadZone;
            speed = 1 * (speed / 4);

            if (speed < 1)
            {
                speed = 1;
            }
            else if (speed >= 5)
            {
                speed = 5;
            }
            //Debug.Log (speed);

            newCamRot.x = newCamRot.x + (Time.deltaTime * sensitivity * speed);

            //Debug.Log ("Over Lower X Limit");
        }
        //END OF X AXIS UPDATE ------------

        //Set y deadzone boundaries
        float yUpperDeadZone = deadZoneYCenter + deadZoneBorder;
        float yLowerDeadZone = deadZoneYCenter - deadZoneBorder;

        if (myoYRot <= yLowerDeadZone)
        {
            //The farther the arm moves from dead zone the faster the camera moves with it
            float speed = yLowerDeadZone - myoYRot;
            speed = 1 * (speed / 4);

            if (speed < 1)
            {
                speed = 1;
            }
            else if (speed >= 5)
            {
                speed = 5;
            }
            //Debug.Log (speed);

            newCamRot.y = newCamRot.y - (Time.deltaTime * sensitivity * speed);

            //Debug.Log ("Over Upper X Limit");
        }
        else if (myoYRot >= yUpperDeadZone)
        {
            //The farther the arm moves from dead zone the faster the camera moves with it
            float speed = myoYRot - yUpperDeadZone;
            speed = 1 * (speed / 4);

            if (speed < 1)
            {
                speed = 1;
            }
            else if (speed >= 5)
            {
                speed = 5;
            }
            //Debug.Log (speed);

            newCamRot.y = newCamRot.y + (Time.deltaTime * sensitivity * speed);

            //Debug.Log ("Over Lower X Limit");
        }

        //
    }

    void SetNewCameraRot()
    {
        Vector3 tempRot = newCamRot;
        tempRot.x = 0;
        tempRot.z = 0;

        //Rotate player body on the y axis instead of the camera
        playerObj.transform.localEulerAngles = tempRot;

        tempRot = newCamRot;
        tempRot.y = 0;

        newCamRot.x = Mathf.Clamp (newCamRot.x, -minCameraXAngle, maxCameraXAngle);

        cam.transform.localEulerAngles = tempRot;
    }

    void Calibrate()
    {
        //Get myo XY axis angles at calibration time
        myoXAtCalibration = myo.transform.localRotation.eulerAngles.x;
        myoYAtCalibration = myo.transform.localRotation.eulerAngles.y;

        //Set dead zone centers
        deadZoneXCenter = myoXAtCalibration;
        deadZoneYCenter = myoYAtCalibration;

        if(deadZoneXCenter > 270 && deadZoneXCenter <= 360)
        {
            deadZoneXCenter -= 360;
        }
    }
	
}
