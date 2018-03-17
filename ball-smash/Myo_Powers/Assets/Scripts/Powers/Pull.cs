using UnityEngine;
using System.Collections;

public class Pull : MonoBehaviour {

    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    [Header ("Settings")]
    public GameObject pointer;
    public GameObject pullStart;
    public float pullRange;
    public float pullMaxStrength;
    public float pullMinStrength;
    public float strengthMaxPercentReductionAtRayEdges;
    public float maxPullRadius;
    public LayerMask layersToCheckForPullPower;

    private float strengthFallOffRate;
    private float strengthFallOffRateCircular;

    private float currentStrength = 8;

    // Use this for initialization
    void Start()
    {
        MyoPoseCheck.onUsePull += UsePull;

        //Range of strength = maxStrength allowed - weakest strength allowed for the pull power
        float rangeOfStrength = pullMaxStrength - pullMinStrength;

        //Find out how much strength decreases for every 1cm along the pull power travel path ( pullRange is converted from m to cm)
        strengthFallOffRate = rangeOfStrength / (pullRange * 100);
    }

    void UsePull()
    {
        //SphereCastAll checks all colliders in its path in a radius 
        RaycastHit[] ray = Physics.SphereCastAll (pullStart.transform.position, maxPullRadius, GetDirection (), pullRange, layersToCheckForPullPower);

        Vector3 startPullPos = pullStart.transform.position;

        int i = 0;

        Vector3 pos = Vector3.zero;

        foreach (RaycastHit hit in ray)
        {
            i++;

            GameObject thisObj = hit.collider.gameObject;

            //Get distance of how much the pull power travelled to reach current gameObject
            float distToHitObj = hit.distance;

            if (i == 1)
            {
                pos = thisObj.transform.position;
            }

            //Get distance from x,y of raycast to x,y of object hit ( to make a nice circular looking pull, rather than applying the same force to all the hit objects)
            float distToXYCentre = Vector3.Distance (pos, thisObj.transform.position);

            //Reduce pull power strength using FallOff modifier found in Start() methods and distance of object from pull source
            currentStrength = pullMaxStrength - (distToHitObj * 100 * strengthFallOffRate);

            strengthFallOffRateCircular = (currentStrength / 100 * strengthMaxPercentReductionAtRayEdges) / (maxPullRadius * 100);

            currentStrength = currentStrength - (distToXYCentre * 100 * strengthFallOffRateCircular);
            //Debug.Log (currentStrength);

            //Disable kinematic mode and add force in the correct direction
            //thisObj.GetComponent<Rigidbody> ().isKinematic = false;
            thisObj.GetComponent<Rigidbody> ().AddForce (GetDirection () * currentStrength);
        }


    }

    Vector3 GetDirection()
    {
        Vector3 dir = pointer.transform.position - pullStart.transform.position;
        return dir;
    }
}
