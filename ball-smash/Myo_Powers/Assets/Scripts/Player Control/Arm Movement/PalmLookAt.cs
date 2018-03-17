using UnityEngine;
using System.Collections;

public class PalmLookAt : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    [Header ("Settings")]
    public Transform target;
    public Transform palmBone;

    private bool canLookAt = false;
    private float defaultY = 0;

    private Vector3 newRot;

    void Start()
    {
        MyoPoseCheck.onUseLightning += LookAtTarget;
        MyoPoseCheck.onStopLightning += StopLookAtTarget;

        defaultY = palmBone.localEulerAngles.y;
        newRot.x = palmBone.localEulerAngles.x;
        newRot.z = palmBone.localEulerAngles.z;
    }

    void Update()
    {
        if(canLookAt)
        {
            palmBone.LookAt (target);
            newRot.y = palmBone.localEulerAngles.y + 60;
            palmBone.localEulerAngles = newRot;
        }
    }

    void LookAtTarget()
    {
        canLookAt = true;
    }

    void StopLookAtTarget()
    {
        canLookAt = false;

        newRot.y = defaultY;
        palmBone.localEulerAngles = newRot;

    }
}
