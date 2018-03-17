using UnityEngine;
using System.Collections;

public class MyoFunctions : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    static ThalmicMyo myo;

    void Start()
    {
        myo = GetComponent<ThalmicMyo> ();
    }

    public static bool IsMyoInXRotationRange(float xStart, float xEnd)
    {
        if(myo.transform.localRotation.eulerAngles.x > xStart && myo.transform.localRotation.eulerAngles.x < xEnd)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
