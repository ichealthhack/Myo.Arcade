using UnityEngine;
using System.Collections;

public class Push : MonoBehaviour {

    /* 
    Klaudijus Miseckas 
    SID:1334116 
    */

    [Header ("Settings")]
    public GameObject pointer;
    public GameObject pushStart;
    public float pushRange;
    public float pushMaxStrength;
    public float pushMinStrength;
    public float strengthMaxPercentReductionAtRayEdges;
    public float maxPushRadius;
    public LayerMask layersToCheckForPushPower;

    private float strengthFallOffRate;
    private float strengthFallOffRateCircular;

    private float currentStrength = 8;

	// Use this for initialization
	void Start ()
    {
        MyoPoseCheck.onUsePush += UsePush;

        //Range of strength = maxStrength allowed - weakest strength allowed for the push power
        float rangeOfStrength = pushMaxStrength - pushMinStrength;

        //Find out how much strength decreases for every 1cm along the push power travel path ( pushRange is converted from m to cm)
        strengthFallOffRate = rangeOfStrength / (pushRange * 100);
	}

    void UsePush()
    {
        //SphereCastAll checks all colliders in its path in a radius 
        RaycastHit[] ray = Physics.SphereCastAll (pushStart.transform.position, maxPushRadius, GetDirection (), pushRange, layersToCheckForPushPower);

        Vector3 startPushPos = pushStart.transform.position;

        int i = 0;

        Vector3 pos = Vector3.zero;

        foreach(RaycastHit hit in ray)
        {
            i++;

            GameObject thisObj = hit.collider.gameObject;

            //Get distance of how much the push power travelled to reach current gameObject
            float distToHitObj = hit.distance;

            if(i == 1)
            {
                pos = thisObj.transform.position;
            }

            //Get distance from x,y of raycast to x,y of object hit ( to make a nice circular looking push, rather than applying the same force to all the hit objects)
            float distToXYCentre = Vector3.Distance (pos, thisObj.transform.position);

            //Reduce push power strength using FallOff modifier found in Start() methods and distance of object from push source
            currentStrength = pushMaxStrength - (distToHitObj * 100 * strengthFallOffRate);

            strengthFallOffRateCircular = (currentStrength / 100 * strengthMaxPercentReductionAtRayEdges) / (maxPushRadius * 100);

            currentStrength = currentStrength - (distToXYCentre * 100 * strengthFallOffRateCircular);
            //Debug.Log (currentStrength);

            //Disable kinematic mode and add force in the correct direction
            //thisObj.GetComponent<Rigidbody> ().isKinematic = false;
            thisObj.GetComponent<Rigidbody> ().AddForce (GetDirection() * currentStrength);
        }


    }

    Vector3 GetDirection()
    {
        Vector3 dir = pointer.transform.position - pushStart.transform.position;
        return dir;
    }
}
