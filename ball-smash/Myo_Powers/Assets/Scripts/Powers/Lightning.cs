using UnityEngine;
using System.Collections;

public class Lightning : MonoBehaviour
{
    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    public DynamicLightning[] lightningScripts;

    void Start()
    {
        MyoPoseCheck.onUseLightning += UseLightning;
        MyoPoseCheck.onStopLightning += StopUsingLightning;
    }

    void UseLightning()
    {
        //Debug.Log ("LIGHTNING ACTIVATED -------------------");

        foreach(DynamicLightning script in lightningScripts)
        {
            script.StartLightning ();
        }

    }

    void StopUsingLightning()
    {
        //Debug.Log ("LIGHTNING STOPPED ---------------------");

    }

}
